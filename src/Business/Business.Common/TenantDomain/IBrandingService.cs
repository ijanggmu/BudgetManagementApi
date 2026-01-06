using System.Threading;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBrandingService
{
    Task<Result<BrandingResponseDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<BrandingResponseDto>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> UpdateAsync(UpdateBrandingDto dto, CancellationToken cancellationToken = default);
}


