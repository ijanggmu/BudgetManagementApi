using Models.Common;
using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.Common.JobHelper.CustomerPolicyJob;

public interface ICustomerPolicyCreateJobService
{
    Task<Result<MessageResponseModel>> CreatePolicyAsync(InsuranceType insuranceType, string transactionId, CancellationToken ct);
}
