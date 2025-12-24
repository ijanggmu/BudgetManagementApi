using Models.BeemaEdgeApi.Branch;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBranchService
{
    Task<Result<List<BranchResponseDto>>> GetAllAsync(string? tenantId = null, CancellationToken cancellationToken = default);
    Task<Result<BranchResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<BranchResponseDto>> CreateAsync(CreateBranchDto dto, CancellationToken cancellationToken = default);
    Task<Result<BranchResponseDto>> UpdateAsync(string id, UpdateBranchDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

