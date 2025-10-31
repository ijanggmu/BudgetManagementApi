using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.WebApi.Customer.Policy;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Kyc;

public interface ICustomerKycService
{
    Task<Result<MessageResponseModel>> SetKycAsync(SetKycRequestModel requestModel);
    Task<Result<GetKycResponseModel>> GetKycAsync();
}
