using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.TwoFactor;

public interface ICustomerTwoFactorService
{
    Task<Result<TwoFaResponseModel>> Set2FaAsync();
    Task<Result<TwoFaValidateResponseModel>> ValidateTotpCodeAsync(string code);
    Task<Result<MessageResponseModel>> Disable2FaAsync();
    Task<Result<List<UserTotpBackUpCodeResponseModel>>> Generate2FaBackUpCodesAsync();
    Task<Result<List<UserTotpBackUpCodeResponseModel>>> GetAll2FaBackUpCodesAsync();
}
