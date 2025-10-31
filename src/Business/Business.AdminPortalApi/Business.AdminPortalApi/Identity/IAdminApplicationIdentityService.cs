using Business.Common.Otp;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Identity;

public interface IAdminApplicationIdentityService
{
    Task<Result<LoginCustomerResponseModel>> LoginAsync(IndividualLoginRequestModel requestModel);
    Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaCustomerRequestModel requestModel);
    Task<Result<MessageResponseModel>> VerifyAdminOtpAsync(VerifyCustomerOtpRequestModel requestModel);
    Task<Result<MessageResponseModel>> RefreshTokenAsync();
    Task<Result<MessageResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> ForgetPasswordAsync(ForgetPasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> SetPasswordAsync(ChangePasswordRequestModel requestModel);
    Task<Result<UserProfileResponseModel>> GetProfileAsync();
    Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel);
    Task<Result<TwoFaResponseModel>> Set2FaAsync();
    Task<Result<MessageResponseModel>> ValidateTotpCodeAsync(string code);
    Task<Result<MessageResponseModel>> Disable2FaAsync();
    Task<Result<List<UserTotpBackUpCodeResponseModel>>> Generate2FaBackUpCodesAsync();
    Task<Result<List<UserTotpBackUpCodeResponseModel>>> GetAll2FaBackUpCodesAsync();
}
