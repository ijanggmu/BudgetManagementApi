using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Fodo;

namespace BeemaEdgeApi.Controllers.V1.MarketingExecutive.MarketingExecutiveRegistrationController;

[AllowAnonymous]
public class MarketingExecutiveRegistrationController(IFodoRegistrationService fodoRegistrationService) : BaseMarketingExecutiveApiController
{
    /// <summary>
    /// Register a new fodo (Field Officer/Door Office Marketing)
    /// </summary>
    /// <param name="requestModel">Fodo registration data</param>
    /// <returns>Success message with OTP sent notification</returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterFodoRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoRegistrationService.RegisterAsync(requestModel, cancellationToken));

    /// <summary>
    /// Verify OTP for fodo registration
    /// </summary>
    /// <param name="requestModel">OTP verification data</param>
    /// <returns>Success message with authentication token</returns>
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyFodoOtpRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoRegistrationService.VerifyFodoOtpAsync(requestModel, cancellationToken));
}

