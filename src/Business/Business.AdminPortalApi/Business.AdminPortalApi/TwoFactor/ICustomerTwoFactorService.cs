using System.Threading;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.TwoFactor;

public interface IAdminTwoFactorService
{
    Task<Result<TwoFaResponseModel>> Set2FaAsync(CancellationToken cancellationToken = default);
    Task<Result<TwoFaValidateResponseModel>> ValidateTotpCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> Disable2FaAsync(CancellationToken cancellationToken = default);
    Task<Result<List<UserTotpBackUpCodeResponseModel>>> Generate2FaBackUpCodesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<UserTotpBackUpCodeResponseModel>>> GetAll2FaBackUpCodesAsync(CancellationToken cancellationToken = default);
}
