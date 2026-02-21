using Models.BeemaEdgeApi.UserSignature;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IUserSignatureService
{
    Task<Result<UserSignatureResponseDto>> GetCurrentUserSignatureAsync(CancellationToken cancellationToken = default);
    Task<Result<UserSignatureResponseDto>> SetSignatureUrlAsync(string signatureUrl, CancellationToken cancellationToken = default);
}
