using System.Threading;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.AdminPassword;

public interface IAdminPasswordService
{
    Task<Result<MessageResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> ForgetPasswordAsync(ForgetPasswordRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> SetPasswordAsync(ChangePasswordRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> ResetPasswordWithOtpAsync(ResetPasswordRequestModel requestModel, CancellationToken cancellationToken = default);
}

