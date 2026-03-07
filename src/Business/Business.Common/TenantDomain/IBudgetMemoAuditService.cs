using Models.BeemaEdgeApi.BudgetMemoAudit;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IBudgetMemoAuditService
{
    Task LogAsync(string entityType, string entityId, string action, string? details = null, CancellationToken cancellationToken = default);
    Task<Result<List<BudgetMemoAuditItemDto>>> GetListAsync(BudgetMemoAuditListRequestModel request, CancellationToken cancellationToken = default);
}
