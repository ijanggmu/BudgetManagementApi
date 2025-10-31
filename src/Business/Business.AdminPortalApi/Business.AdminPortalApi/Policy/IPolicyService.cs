using Models.Common;
using Models.Common.Policy.Manufacturer;
using Models.Common.Policy.Policy;
using Models.Common.Policy.ThirdPartyApi;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Policy;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Policy;
public interface IPolicyService
{
    Task<Result<IEnumerable<PolicyResponseModel>>> GetMyPoliciesAsync(CommonPaginationRequestModel requestModel);
    Task<Result<PolicyResponseModel>> BuyPolicyAsync(BuyPolicyRequestModel buyPolicyDto);
    Task<Result<PolicyResponseModel>> RenewPolicyAsync(BuyPolicyRequestModel renewPolicyDto);
    Task<Result<ThirdPartyPolicyResponseModel>> GetPolicyDetailsAsync(string draftNo);
    Task<string> PrintPolicyAsync(PrintPolicyCoreRequestModel requestModel);
    Task<Result<List<PolicyIssuanceViewModel>>> GetPolicyList(string partyCode);
    Task<Result<List<ManufacturerImportExcelData>>> GetAllManufacture(ManufactureByClassNameRequestModel requestModel);
    Task<Result<List<PolicyDetailResponse>>> PolicyDetails(PolicyDetailRequest model);
}
