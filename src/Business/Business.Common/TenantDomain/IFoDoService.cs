using Models.BeemaEdgeApi.Fodo;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IFodoService
{
    Task<Result<List<FodoResponseDto>>> GetFodosForAdminAsync(string? tenantId = null, CancellationToken cancellationToken = default);
    Task<Result<FodoResponseDto>> GetFodoByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<FodoResponseDto>> CreateAsync(CreateFodoDto dto, CancellationToken cancellationToken = default);
    Task<Result<FodoResponseDto>> UpdateAsync(string id, UpdateFodoDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Action methods
    Task<Result<bool>> ChangePasswordAsync(string id, string newPassword, CancellationToken cancellationToken = default);
    Task<Result<bool>> ToggleUserStatusAsync(string id, bool isDisabled, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetAccessLogsAsync(string id, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetLeadsAsync(string id, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetQuotationsAsync(string id, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
}

