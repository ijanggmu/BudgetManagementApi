
using Models.BeemaEdgeApi.Fodo;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IFodoService
{
    Task<Result<List<FodoResponseDto>>> GetFodosForAdminAsync(string? tenantId = null, CancellationToken cancellationToken = default);
    Task<Result<FodoResponseDto>> GetFodoByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<FodoResponseDto>> CreateAsync(CreateFodoDto dto, CancellationToken cancellationToken = default);
    Task<Result<FodoResponseDto>> UpdateAsync(string id, UpdateFodoDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

