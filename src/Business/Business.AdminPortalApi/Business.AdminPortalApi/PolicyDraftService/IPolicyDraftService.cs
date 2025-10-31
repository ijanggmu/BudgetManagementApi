using Models.Common;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Customer.Policy;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.PolicyDraftService;
public interface IPolicyDraftService
{
    Task<Result<MessageResponseModel>> SaveDraftAsync(SaveDraftRequestModel model);


}
