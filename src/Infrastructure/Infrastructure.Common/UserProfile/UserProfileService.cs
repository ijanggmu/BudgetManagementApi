using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Models.Common.Token;
using SharedKernel.Constant;

namespace Infrastructure.Common.UserProfile;

public class UserProfileService : IUserProfileService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _webHostEnvironment;

    public UserProfileService(IHttpContextAccessor httpContextAccessor,
        IHostEnvironment webHostEnvironment)
    {
        _httpContextAccessor = httpContextAccessor;
        _webHostEnvironment = webHostEnvironment;

    }

    public string GetIpAddress()
    {
        var ip = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = _httpContextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        return ip;
    }
    public string GetScheme()
    {
        var scheme = _httpContextAccessor.HttpContext.Request.Headers["X-Original-Scheme"].FirstOrDefault();

        if (string.IsNullOrEmpty(scheme))
        {
            scheme = _httpContextAccessor.HttpContext.Request.Scheme;
        }
        return scheme;
    }
    public LoggingUserObject GetUser()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return new LoggingUserObject
            {
                IpAddress = GetIpAddress(),
            };
        }
        else
        {
            var claimsIdentity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            if (claimsIdentity != null && claimsIdentity.FindFirst(TokenKey.UserId) != null)
            {
                return new LoggingUserObject
                {
                    IpAddress = GetIpAddress(),
                    UserId = claimsIdentity.FindFirst(TokenKey.UserId).Value,
                    Username = claimsIdentity.FindFirst(TokenKey.Username).Value,
                };
            }
            else
            {
                return new LoggingUserObject
                {
                    IpAddress = GetIpAddress(),
                };
            }
        }
    }

    public string GetUserId()
    {
        var claimsIdentity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
        if (claimsIdentity != null && claimsIdentity.FindFirst(TokenKey.UserId) != null)
            return claimsIdentity.FindFirst(TokenKey.UserId).Value;
        return string.Empty;
    }

    public string GetUsername()
    {
        var claimsIdentity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
        if (claimsIdentity != null && claimsIdentity.FindFirst(TokenKey.Username) != null)
            return claimsIdentity.FindFirst(TokenKey.Username).Value;
        return string.Empty;
    }

    public string GetUserTimeZone()
    {
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Timezone-Offset", out var timezoneOffset))
            return timezoneOffset;

        return timezoneOffset;
    }
    public string GetAccessToken()
    {
        if (_httpContextAccessor.HttpContext?.Request.Cookies == null)
            return null;

        var token = _httpContextAccessor.HttpContext.Request.Cookies["X-Access-Token"];
        if (string.IsNullOrEmpty(token))
            return null;

        return HttpUtility.UrlDecode(token);
    }

    public string GetRefreshToken()
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["X-Refresh-Token"];
        if (token == null)
            return null;
        return HttpUtility.UrlDecode(HttpUtility.UrlDecode(token));
    }
    public void SetAuthCookiesInClient(TokenModel tokenModel, string username)
    {
        var environmentName = _webHostEnvironment.EnvironmentName;
        Console.WriteLine("environmentname:@env", environmentName);

        if (_webHostEnvironment.IsDevelopment() || environmentName.ToLower() == "dev")
        {
            SetAuthCookiesInDevelopmentClient(HttpUtility.UrlEncode(tokenModel.AccessToken), tokenModel.AccessTokenExpiryInSeconds, username, HttpUtility.UrlEncode(tokenModel.RefreshToken), tokenModel.RefreshTokenExpiryInSeconds);
        }
        else
        {
            SetAuthCookiesInClient(HttpUtility.UrlEncode(tokenModel.AccessToken), tokenModel.AccessTokenExpiryInSeconds, username, HttpUtility.UrlEncode(tokenModel.RefreshToken), tokenModel.RefreshTokenExpiryInSeconds);
        }
    }

    public void SetAuthCookiesInClient(string accessToken, int accessTokenExpiryInSeconds, string userName, string refreshToken, int refreshTokenExpiryInSeconds)
    {
        var expirytimeAccess = DateTimeOffset.UtcNow.AddSeconds(accessTokenExpiryInSeconds);
        var expirytimeRefresh = DateTimeOffset.UtcNow.AddSeconds(refreshTokenExpiryInSeconds);

        var isLocalhost = _httpContextAccessor.HttpContext.Request.Host.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase);

        _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Access-Token", HttpUtility.UrlEncode(accessToken),
                                                                                          new CookieOptions()
                                                                                          {
                                                                                              HttpOnly = true,
                                                                                              SameSite = SameSiteMode.Strict,
                                                                                              Secure = true,
                                                                                              Expires = expirytimeAccess
                                                                                          });

        _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Access-Token-ExpiryInSeconds", accessTokenExpiryInSeconds.ToString(),
                                                                                                new CookieOptions()
                                                                                                {
                                                                                                    HttpOnly = true,
                                                                                                    SameSite = SameSiteMode.Strict,
                                                                                                    Secure = true,
                                                                                                    Expires = expirytimeAccess
                                                                                                });
        _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Username", userName,
                                                                new CookieOptions()
                                                                {
                                                                    HttpOnly = true,
                                                                    SameSite = SameSiteMode.Strict,
                                                                    Secure = true,
                                                                    Expires = expirytimeRefresh
                                                                });

        _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Refresh-Token", HttpUtility.UrlEncode(refreshToken),
                                                                    new CookieOptions()
                                                                    {
                                                                        HttpOnly = true,
                                                                        SameSite = SameSiteMode.Strict,
                                                                        Secure = true,
                                                                        Expires = expirytimeRefresh
                                                                    });

        _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Refresh-ExpiryInSeconds", refreshTokenExpiryInSeconds.ToString(),
                                                                    new CookieOptions()
                                                                    {
                                                                        HttpOnly = true,
                                                                        SameSite = SameSiteMode.Strict,
                                                                        Secure = true,
                                                                        Expires = expirytimeRefresh
                                                                    });
    }

    public void SetAuthCookiesInDevelopmentClient(
      string accessToken,
      int accessTokenExpiryInSeconds,
      string userName,
      string refreshToken,
      int refreshTokenExpiryInSeconds)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var expiryAccess = DateTimeOffset.UtcNow.AddSeconds(accessTokenExpiryInSeconds);
        var expiryRefresh = DateTimeOffset.UtcNow.AddSeconds(refreshTokenExpiryInSeconds);

        var baseOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            // Domain = "localhost" // if using subdomains
        };
        Console.WriteLine(baseOptions.Domain);

        void AppendCookie(string key, string value, DateTimeOffset expiry)
        {
            var options = new CookieOptions
            {
                HttpOnly = baseOptions.HttpOnly,
                Secure = baseOptions.Secure,
                SameSite = baseOptions.SameSite,
                Expires = expiry,
            };
            httpContext.Response.Cookies.Append(key, value, options);
        }

        AppendCookie("X-Access-Token", HttpUtility.UrlEncode(accessToken), expiryAccess);
        AppendCookie("X-Access-Token-ExpiryInSeconds", accessTokenExpiryInSeconds.ToString(), expiryAccess);
        AppendCookie("X-Username", userName, expiryRefresh);
        AppendCookie("X-Refresh-Token", HttpUtility.UrlEncode(refreshToken), expiryRefresh);
        AppendCookie("X-Refresh-ExpiryInSeconds", refreshTokenExpiryInSeconds.ToString(), expiryRefresh);
    }

    public string GetRoleId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var claimsIdentity = httpContext.User?.Identity as ClaimsIdentity;
            if (claimsIdentity != null && claimsIdentity.FindFirst(TokenKey.RoleId) != null)
            {
                return claimsIdentity.FindFirst(TokenKey.RoleId).Value;
            }
        }
        return string.Empty;
    }
    public void RemoveAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete("X-Access-Token");
        response.Cookies.Delete("X-Access-Token-ExpiryInSeconds");
        response.Cookies.Delete("X-Username");
        response.Cookies.Delete("X-Refresh-Token");
        response.Cookies.Delete("X-Refresh-ExpiryInSeconds");
    }
    public void SetUser(string userId, string username)
    {
        var claims = new List<Claim>
    {
        new Claim(TokenKey.UserId, userId),
        new Claim(TokenKey.Username, username)
    };

        var identity = new ClaimsIdentity(claims, "Custom"); // <<< important: scheme name
        var principal = new ClaimsPrincipal(identity);

        _httpContextAccessor.HttpContext.User = principal;
    }

}
