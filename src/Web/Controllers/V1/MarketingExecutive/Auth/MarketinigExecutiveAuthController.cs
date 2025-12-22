using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Auth;

public class MarketinigExecutiveAuthController(IFodoAuthService fodoAuthService) : BaseMarketingExecutiveApiController
{
    /// <summary>
    /// Fodo (Field Officer/Door Office Marketing) login
    /// </summary>
    /// <param name="requestModel">Login credentials</param>
    /// <returns>Login response with token or OTP requirement</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AgentLoginRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoAuthService.LoginAsync(requestModel, cancellationToken));

    /// <summary>
    /// Verify 2FA code for fodo login
    /// </summary>
    /// <param name="requestModel">2FA verification data</param>
    /// <returns>Success message with token</returns>
    [AllowAnonymous]
    [HttpPost("login-2fa")]
    public async Task<IActionResult> Login2FaAsync([FromBody] Verify2FaCustomerRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoAuthService.Login2FaAsync(requestModel, cancellationToken));

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <returns>New token pair</returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken = default)
        => HandleResult(await fodoAuthService.RefreshTokenAsync(cancellationToken));

    /// <summary>
    /// Fodo logout
    /// </summary>
    /// <returns>Success message</returns>
    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
        => HandleResult(fodoAuthService.Logout(HttpContext.Response));
}

