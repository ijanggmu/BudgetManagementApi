using Models.BeemaEdgeApi.Budget;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBudgetService
{
    Task<Result<List<BudgetResponseDto>>> GetAllAsync(BudgetListRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<BudgetResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<BudgetResponseDto>> CreateAsync(CreateBudgetDto dto, CancellationToken cancellationToken = default);
    Task<Result<BudgetResponseDto>> UpdateAsync(string id, UpdateBudgetDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
