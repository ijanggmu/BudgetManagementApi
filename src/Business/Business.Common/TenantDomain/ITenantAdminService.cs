using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface ITenantAdminService
{
    Task<Result<List<TenantsResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default);
    Task<Result<TenantResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<TenantsResponseDto>> CreateAsync(CreateTenantDto dto, CancellationToken cancellationToken = default);
    Task<Result<TenantsResponseDto>> UpdateAsync(string id, UpdateTenantDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<TenantDropdownDto>>> GetTenantsForDropdownAsync(CancellationToken cancellationToken = default);
    Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
}
