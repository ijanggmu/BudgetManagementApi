using Data.Entities.Identity;

namespace Business.Common.Token;

public interface ITokenService
{
    Tuple<string, int> CreateToken(ApplicationUser user, List<string> roleIds);
    Task<Tuple<string, int>> CreateRefreshToken(ApplicationUser user);
    Tuple<string, string> GetUserAndRoleFromAccessToken(string token);
}
