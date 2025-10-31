using Models;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerClaim;
using Models.WebApi.Claim;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Claim;

public interface ICustomerClaimService
{
    Task<Result<List<CustomerClaimResponseModel>>> GetCustomerClaimAsync(CommonPaginationRequestModel requestModel);
    Task<Result<CustomerClaimResponseModel>> GetCustomerClaimByIdAsync(string id);
    Task<Result<CustomerClaimResponseModel>> ApplyClaimAsync(ClaimIntimationRequestModel model);
    Task<Result<CustomerClaimResponseModel>> UpdateClaimAsync(string id);
    Task<Result<CustomerClaimResponseModel>> GetHospitalNamesAsync(CommonPaginationRequestModel requestModel);
}
