using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.AdminPortalApi.AdminPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Controllers.V1.Admin.Password;

public class AdminPasswordController(IAdminPasswordService adminPasswordService) : BaseAdminApiController
{
    /// <summary>
    /// Change password for authenticated admin user
    /// </summary>
    /// <param name="requestModel">Password change data</param>
    /// <returns>Success message</returns>
    [HttpPut("change")]
    public async Task<IActionResult> ChangeAsync([FromBody] ChangePasswordRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await adminPasswordService.ChangePasswordAsync(requestModel, cancellationToken));

    /// <summary>
    /// Request password reset OTP (forgot password)
    /// </summary>
    /// <param name="requestModel">Username for password reset</param>
    /// <returns>Success message with OTP sent notification</returns>
    [AllowAnonymous]
    [HttpPost("forget")]
    public async Task<IActionResult> ForgetAsync([FromBody] ForgetPasswordRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await adminPasswordService.ForgetPasswordAsync(requestModel, cancellationToken));

    /// <summary>
    /// Set password for user who doesn't have a password yet
    /// </summary>
    /// <param name="requestModel">Password data</param>
    /// <returns>Success message</returns>
    [HttpPost("set")]
    public async Task<IActionResult> SetAsync([FromBody] ChangePasswordRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await adminPasswordService.SetPasswordAsync(requestModel, cancellationToken));

    /// <summary>
    /// Reset password using OTP token
    /// </summary>
    /// <param name="requestModel">Password reset data with OTP token</param>
    /// <returns>Success message</returns>
    [AllowAnonymous]
    [HttpPost("reset")]
    public async Task<IActionResult> ResetAsync([FromBody] ResetPasswordRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await adminPasswordService.ResetPasswordWithOtpAsync(requestModel, cancellationToken));
}

