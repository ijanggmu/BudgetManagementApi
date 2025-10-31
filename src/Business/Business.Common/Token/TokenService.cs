using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Context;
using Data.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Constant;

namespace Business.Common.Token;
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly ApplicationDataContext _context;
    private readonly string _key;

    public TokenService(IConfiguration config, ApplicationDataContext context)
    {
        _config = config;
        _context = context;
        _key = _config["Jwt:Key"];
    }

    public Tuple<string, int> CreateToken(ApplicationUser user, List<string> roleIds)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(TokenKey.UserId, user.Id),
                new Claim(TokenKey.Username, user.UserName),
                new Claim(TokenKey.RoleId, string.Join(',',roleIds)),
            };

        _ = int.TryParse(_config["Jwt:JWTAdminExpiresInMinutes"], out int expiryTime);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryTime),
            SigningCredentials = credentials,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new(tokenHandler.WriteToken(token), expiryTime * 60);
    }


    public async Task<Tuple<string, int>> CreateRefreshToken(ApplicationUser user)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);

        _ = int.TryParse(_config["Jwt:RefreshExpiresInMinutes"], out int refreshTokenValidityInMinutes);
        int tokenTotalTime = refreshTokenValidityInMinutes * 60;

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryDateTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);


        _context.Users.Update(user);

        await _context.SaveChangesAsync();


        return new(refreshToken, tokenTotalTime);
    }

    public Tuple<string, string> GetUserAndRoleFromAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateIssuer = false, 
                ValidateAudience = false,
                ValidateLifetime = false // Disable lifetime validation
            }, out SecurityToken validatedToken);

            var userId = principal.FindFirst(TokenKey.UserId).Value;
            var roleId = principal.FindFirst(TokenKey.RoleId).Value;

            return new(userId, roleId);
        }
        catch
        {
            return new(null, null);
        }
    }
}
