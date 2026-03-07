using Models.BeemaEdgeApi.Budget;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBudgetSubheadingService
{
    Task<Result<List<BudgetSubheadingResponseDto>>> GetAllAsync(
        string? budgetHeadingId = null,
        CancellationToken cancellationToken = default);

    Task<Result<BudgetSubheadingResponseDto>> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<Result<BudgetSubheadingResponseDto>> CreateAsync(
        CreateBudgetSubheadingDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<BudgetSubheadingResponseDto>> UpdateAsync(
        string id,
        UpdateBudgetSubheadingDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(
        string id,
        CancellationToken cancellationToken = default);
}

