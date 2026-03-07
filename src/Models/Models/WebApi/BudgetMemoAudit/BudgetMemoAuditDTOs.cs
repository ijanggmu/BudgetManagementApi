using Models.Common;

namespace Models.BeemaEdgeApi.BudgetMemoAudit;

public class BudgetMemoAuditListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? EntityType { get; set; } // "Budget" | "Memo"
    public string? EntityId { get; set; }
}

public class BudgetMemoAuditItemDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedOn { get; set; }
}
