using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerClaim;
using Models.WebApi.Claim;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Claim;

public interface IAdminClaimService
{
    Task<Result<List<CustomerClaimResponseModel>>> GetCustomerClaimAsync(CommonPaginationRequestModel requestModel);
    Task<Result<CustomerClaimResponseModel>> GetCustomerClaimByIdAsync(string id);
    Task<Result<CustomerClaimResponseModel>> ApplyClaimAsync(ClaimIntimationRequestModel model);
    Task<Result<CustomerClaimResponseModel>> UpdateClaimAsync(string id);
    Task<Result<CustomerClaimResponseModel>> GetHospitalNamesAsync(CommonPaginationRequestModel requestModel);
}
