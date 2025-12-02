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
        if (_httpContextAccessor.HttpContext?.Request == null)
            return string.Empty;

        var ip = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            var remoteIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                ip = remoteIp.MapToIPv4().ToString();
            }
        }

        return ip ?? string.Empty;
    }
    public string GetScheme()
    {
        if (_httpContextAccessor.HttpContext?.Request == null)
            return "https"; // Default to https

        var scheme = _httpContextAccessor.HttpContext.Request.Headers["X-Original-Scheme"].FirstOrDefault();

        if (string.IsNullOrEmpty(scheme))
        {
            scheme = _httpContextAccessor.HttpContext.Request.Scheme;
        }
        return scheme ?? "https";
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
            var userIdClaim = claimsIdentity?.FindFirst(TokenKey.UserId);
            var usernameClaim = claimsIdentity?.FindFirst(TokenKey.Username);

            if (claimsIdentity != null && userIdClaim != null && usernameClaim != null)
            {
                return new LoggingUserObject
                {
                    IpAddress = GetIpAddress(),
                    UserId = userIdClaim.Value,
                    Username = usernameClaim.Value,
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
        var userIdClaim = claimsIdentity?.FindFirst(TokenKey.UserId);
        return userIdClaim?.Value ?? string.Empty;
    }

    public string GetUsername()
    {
        var claimsIdentity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
        var usernameClaim = claimsIdentity?.FindFirst(TokenKey.Username);
        return usernameClaim?.Value ?? string.Empty;
    }

    public string GetUserTimeZone()
    {
        if (_httpContextAccessor.HttpContext?.Request.Headers != null &&
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Timezone-Offset", out var timezoneOffset))
        {
            return timezoneOffset.ToString();
        }

        return string.Empty;
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
        if (_httpContextAccessor.HttpContext?.Request.Cookies == null)
            return null;

        var token = _httpContextAccessor.HttpContext.Request.Cookies["X-Refresh-Token"];
        if (string.IsNullOrEmpty(token))
            return null;
        return HttpUtility.UrlDecode(token);
    }
    public void SetAuthCookiesInClient(TokenModel tokenModel, string username)
    {
        var environmentName = _webHostEnvironment.EnvironmentName;

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
        var isDevelopment = _webHostEnvironment.IsDevelopment() || _webHostEnvironment.EnvironmentName.ToLower() == "dev";

        // In development/localhost, use Lax SameSite and conditional Secure flag
        var sameSite = (isDevelopment && isLocalhost) ? SameSiteMode.Lax : SameSiteMode.Strict;
        var secure = !isLocalhost; // Only use Secure flag for non-localhost

        void AppendCookie(string key, string value, DateTimeOffset expiry)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                HttpOnly = true,
                SameSite = sameSite,
                Secure = secure,
                Expires = expiry,
                Path = "/"
            });
        }

        AppendCookie("X-Access-Token", HttpUtility.UrlEncode(accessToken), expirytimeAccess);
        AppendCookie("X-Access-Token-ExpiryInSeconds", accessTokenExpiryInSeconds.ToString(), expirytimeAccess);
        AppendCookie("X-Username", userName, expirytimeRefresh);
        AppendCookie("X-Refresh-Token", HttpUtility.UrlEncode(refreshToken), expirytimeRefresh);
        AppendCookie("X-Refresh-ExpiryInSeconds", refreshTokenExpiryInSeconds.ToString(), expirytimeRefresh);
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

        var isLocalhost = httpContext.Request.Host.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase);

        // For development: use Lax for localhost, None for cross-origin (but only if needed)
        // Secure flag should be false for localhost http, true for https
        var sameSite = isLocalhost ? SameSiteMode.Lax : SameSiteMode.None;
        var secure = !isLocalhost; // Only use Secure for non-localhost

        void AppendCookie(string key, string value, DateTimeOffset expiry)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = sameSite,
                Expires = expiry,
                Path = "/"
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
            var roleIdClaim = claimsIdentity?.FindFirst(TokenKey.RoleId);
            return roleIdClaim?.Value ?? string.Empty;
        }
        return string.Empty;
    }
    public void RemoveAuthCookies(HttpResponse response)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        };

        response.Cookies.Append("X-Access-Token", string.Empty, cookieOptions);
        response.Cookies.Append("X-Access-Token-ExpiryInSeconds", string.Empty, cookieOptions);
        response.Cookies.Append("X-Username", string.Empty, cookieOptions);
        response.Cookies.Append("X-Refresh-Token", string.Empty, cookieOptions);
        response.Cookies.Append("X-Refresh-ExpiryInSeconds", string.Empty, cookieOptions);
    }
    public void SetUser(string userId, string username)
    {
        if (_httpContextAccessor.HttpContext == null)
            return;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            return;

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
