using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface ITenantAdminService
{
    Task<Result<List<TenantsResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null);
    Task<Result<TenantResponseDto>> GetByIdAsync(string id);
    Task<Result<TenantsResponseDto>> CreateAsync(CreateTenantDto dto);
    Task<Result<TenantsResponseDto>> UpdateAsync(string id, UpdateTenantDto dto);
    Task<Result<bool>> DeleteAsync(string id);
    Task<Result<List<TenantDropdownDto>>> GetTenantsForDropdownAsync();
    Task<Result<byte[]>> ExportToExcelAsync();
}
