using System.Threading.Tasks;
using Business.AdminPortalApi.Auth;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Controllers.V1.Admin.Auth
{
    [AllowAnonymous]
    public class AdminAuthController(IAdminAuthService adminAuthService) : BaseAdminApiController
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestModel requestModel) => HandleResult(await adminAuthService.LoginAsync(requestModel));

        [HttpPost("Login2FA")]
        public async Task<IActionResult> Login2FA([FromBody] Verify2FaAdminRequestModel requestModel) => HandleResult(await adminAuthService.Login2FaAsync(requestModel));

        [HttpGet("Refresh")]
        public async Task<IActionResult> RefreshToken() => HandleResult(await adminAuthService.RefreshTokenAsync());

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("X-Access-Token");
            HttpContext.Response.Cookies.Delete("X-Access-Token-ExpiryInSeconds");
            HttpContext.Response.Cookies.Delete("X-Username");
            HttpContext.Response.Cookies.Delete("X-Refresh-Token");
            HttpContext.Response.Cookies.Delete("X-Refresh-ExpiryInSeconds");
            return Ok("Logged out successfully.");
        }
    }
}

