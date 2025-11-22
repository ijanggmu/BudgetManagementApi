using Models.BeemaEdgeApi.Fodo;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IFodoService
{
    Task<Result<List<FodoResponseDto>>> GetFodosForAdminAsync(string? tenantId = null);
    Task<Result<FodoResponseDto>> GetFodoByIdAsync(string id);
    Task<Result<FodoResponseDto>> CreateAsync(CreateFodoDto dto);
    Task<Result<FodoResponseDto>> UpdateAsync(string id, UpdateFodoDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}

