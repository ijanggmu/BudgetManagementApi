using Models.Common.Policy.ThirdPartyApi;
using SharedKernel.Operation;

namespace Business.Common.Policy;

public interface IPolicyAcknowlegeService
{
    Task<Result<string>> AcknowledgeCreationAndStoreDetailsAsync(ThirdPartyPolicyCreateResponse responseModel);
}
