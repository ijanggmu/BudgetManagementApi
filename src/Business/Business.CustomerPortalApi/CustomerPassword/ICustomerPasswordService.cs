using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.CustomerPassword;

public interface ICustomerPasswordService
{
    Task<Result<MessageResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> ForgetPasswordAsync(ForgetPasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> SetPasswordAsync(SetPasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> ResetPasswordAsync(ResetPasswordRequestModel requestModel);
}
