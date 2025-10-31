using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.Otp;

public interface IOtpService
{
    Task<Result<VerifyOtpResponseModel>> VerifyOtpAsync(VerifyOtpRequestModel requestModel);
    Task<Result<MessageResponseModel>> ResendOtpAsync(ResendOtpRequestModel requestModel);
}



