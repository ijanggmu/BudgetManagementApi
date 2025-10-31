using Microsoft.AspNetCore.Http;
using Models.Common.Token;

namespace Infrastructure.Common.UserProfile
{
    public interface IUserProfileService
    {
        string GetUserId();
        string GetUsername();
        string GetUserTimeZone();
        LoggingUserObject GetUser();
        string GetIpAddress();

        string GetScheme();
        string GetAccessToken();

        string GetRefreshToken();
        void SetAuthCookiesInClient(TokenModel tokenModel, string username);
        string GetRoleId();
        void RemoveAuthCookies(HttpResponse response);
        void SetUser(string userId, string username);
    }


    public class LoggingUserObject
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
    }
}
