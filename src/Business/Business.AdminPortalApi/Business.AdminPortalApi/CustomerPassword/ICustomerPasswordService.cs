using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.AdminPassword;

public interface IAdminPasswordService
{
    Task<Result<MessageResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> ForgetPasswordAsync(ForgetPasswordRequestModel requestModel);
    Task<Result<MessageResponseModel>> SetPasswordAsync(ChangePasswordRequestModel requestModel);
}
