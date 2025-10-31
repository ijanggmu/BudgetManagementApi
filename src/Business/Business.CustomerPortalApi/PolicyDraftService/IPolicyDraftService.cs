using Models.Common;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Customer.Policy;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.PolicyDraftService;
public interface IPolicyDraftService
{
    Task<Result<MessageResponseModel>> SaveDraftAsync(SaveDraftRequestModel model, CancellationToken cancellationToken);
    Task<Result<SubmitDraftResponseModel>> SubmitDraftAsync(SubmitDraftRequestModel model, CancellationToken cancellationToken);
    Task<Result<List<GetAllDraftResponseModel>>> GetDraftPoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken);
    Task<Result<PolicyDraftByIdResponseModel>> GetDraftPolicyByIdAsync(string draftId, CancellationToken cancellationToken);
}
