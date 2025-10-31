using Models.Common;
using Models.Common.Policy.Manufacturer;
using Models.Common.Policy.Policy;
using Models.Common.Policy.ThirdPartyApi;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Customer.Policy;
using Models.WebApi.ITI;
using Models.WebApi.Policy;
using Models.WebApi.Policy.Motor;
using SharedKernel.Operation;

namespace Business.Common.Policy;
public interface IPolicyService
{
    Task<Result<ThirdPartyPolicyResponseModel>> GetPolicyDetailsAsync(string draftNo, CancellationToken cancellationToken = default);
    Task<string> PrintPolicyAsync(PrintPolicyRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<string> PrintCertificateAsync(PrintCertificateRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<string> PrintReceiptAsync(PrintReceiptRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<List<PolicyIssuanceViewModel>>> GetPolicyList(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<List<ManufacturerImportExcelData>>> GetAllManufactureByClassName(string className);
    Task<Result<List<ManufacturerViewModel>>> GetAllManufacture(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default); Task<Result<List<PolicyDetailResponse>>> PolicyDetails(PolicyDetailRequest model, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> CreateMotorcyclePolicy(MotorPolicyRequestModel model, string userId, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> CreateITIPolicy(string policyId, CancellationToken cancellationToken = default);
    Task<Result<List<GetAllActivePoliciesResponseModel>>> GetActivePoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken);
    Task<Result<PolicyByIdResponseModel>> GetPolicyByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<List<GetAllActivePoliciesResponseModel>>> GetAllPoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken);
}



