using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Claim;

public interface ICmsCustomerService
{
    Task<Result<List<CustomerResponseModel>>> GetAllCustomersAsync(CommonPaginationRequestModel requestModel, CancellationToken ct);

    Task<Result<GetKycResponseModel>> GetCustomerByIdAsync(string id, CancellationToken ct);
    // Admin approves KYC
    Task<Result<MessageResponseModel>> ApproveCustomerKycAsync(string customerId, CancellationToken ct);

    // Admin rejects KYC with reason
    Task<Result<MessageResponseModel>> RejectCustomerKycAsync(string customerId, string reason, CancellationToken ct);
}

public class CustomerResponseModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public string Username { get; set; }
    public string KycStatus { get; set; }
    public string KycRejectedReason { get; set; }
    public string CreatedOn { get; set; }
}

