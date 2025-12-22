using System.Threading;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Profile;

public interface IAdminProfileService
{
    Task<Result<AdminUserProfileResponseModel>> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel, CancellationToken cancellationToken = default);
}
