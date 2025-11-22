using System.Threading.Tasks;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBrandingService
{
    Task<Result<BrandingResponseDto>> GetAsync();
    Task<Result<BrandingResponseDto>> GetByTenantIdAsync(string tenantId);
    Task<Result<BrandingResponseDto>> UpdateAsync(UpdateBrandingDto dto);
}


