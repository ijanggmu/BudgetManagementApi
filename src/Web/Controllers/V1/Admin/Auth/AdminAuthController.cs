using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.AdminPortalApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Controllers.V1.Admin.Auth
{
    [AllowAnonymous]
    public class AdminAuthController(IAdminAuthService adminAuthService) : BaseAdminApiController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestModel requestModel) => HandleResult(await adminAuthService.LoginAsync(requestModel));

        [HttpPost("login2FA")]
        public async Task<IActionResult> Login2FA([FromBody] Verify2FaAdminRequestModel requestModel) => HandleResult(await adminAuthService.Login2FaAsync(requestModel));

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken() => HandleResult(await adminAuthService.RefreshTokenAsync());

        [HttpPost("logout")]
        public async Task<IActionResult> Logout() => HandleResult(await adminAuthService.LogoutAsync(HttpContext.Response));
    }
}

