using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Profile;

public interface ICustomerProfileService
{
    Task<Result<UserProfileResponseModel>> GetProfileAsync();
    Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel);
}
