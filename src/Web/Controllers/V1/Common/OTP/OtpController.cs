using System.Threading.Tasks;
using Business.Common.Otp;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Common.OTP;
[AllowAnonymous]
public class OtpController(IOtpService otpService) : BaseCommonApiController
{
    [HttpPost("VerifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestModel requestModel)
    {
        var result = await otpService.VerifyOtpAsync(requestModel);
        return HandleResult(result);
    }

    [HttpPost("ResendOtp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestModel requestModel)
    {
        var result = await otpService.ResendOtpAsync(requestModel);
        return HandleResult(result);
    }

}
