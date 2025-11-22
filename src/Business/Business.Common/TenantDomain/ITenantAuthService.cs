using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface ITenantAuthService
{
    Task<Result<TenantLoginResponseDto>> LoginAsync(TenantLoginRequestDto request);
}

