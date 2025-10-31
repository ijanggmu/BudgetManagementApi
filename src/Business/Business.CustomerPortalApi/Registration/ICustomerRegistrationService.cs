using Business.Common.Otp;
using Models.Common;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Registration;
public interface ICustomerRegistrationService
{
    Task<Result<MessageResponseModel>> RegisterAsync(RegisterCustomerRequestModel requestModel);
    Task<Result<MessageResponseModel>> VerifyCustomerOtpAsync(VerifyCustomerOtpRequestModel requestModel);
}
