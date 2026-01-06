using System.Threading;
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
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestModel requestModel, CancellationToken cancellationToken = default)
            => HandleResult(await adminAuthService.LoginAsync(requestModel, cancellationToken));

        [HttpPost("login2FA")]
        public async Task<IActionResult> Login2FA([FromBody] Verify2FaAdminRequestModel requestModel, CancellationToken cancellationToken = default)
            => HandleResult(await adminAuthService.Login2FaAsync(requestModel, cancellationToken));

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken = default)
            => HandleResult(await adminAuthService.RefreshTokenAsync(cancellationToken));

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
            => HandleResult(await adminAuthService.LogoutAsync(HttpContext.Response, cancellationToken));
    }
}

