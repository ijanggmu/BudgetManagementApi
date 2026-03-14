using Models.BeemaEdgeApi.Budget;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBudgetHeadingService
{
    Task<Result<List<BudgetHeadingResponseDto>>> GetAllAsync(BudgetHeadingListRequestModel? requestModel = null, CancellationToken cancellationToken = default);
    Task<Result<BudgetHeadingResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<BudgetHeadingResponseDto>> CreateAsync(CreateBudgetHeadingDto dto, CancellationToken cancellationToken = default);
    Task<Result<BudgetHeadingResponseDto>> UpdateAsync(string id, UpdateBudgetHeadingDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
