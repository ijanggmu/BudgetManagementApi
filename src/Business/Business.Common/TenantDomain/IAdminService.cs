using System.Threading;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IAdminService
{
    Task<Result<List<AdminResponseDto>>> GetAdminsForAdminAsync(CommonPaginationRequestModel requestModel, string? tenantId = null, CancellationToken cancellationToken = default);
    Task<Result<AdminResponseDto>> GetAdminByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<AdminResponseDto>> CreateAsync(CreateAdminDto dto, CancellationToken cancellationToken = default);
    Task<Result<AdminResponseDto>> UpdateAsync(string id, UpdateAdminDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
}

