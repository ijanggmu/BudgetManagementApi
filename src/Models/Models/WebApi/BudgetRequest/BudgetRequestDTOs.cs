using Models.Common;

namespace Models.BeemaEdgeApi.BudgetRequest;

public class BudgetRequestResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // PendingApproval, Approved, Rejected
    public string? NextApproverRoleId { get; set; }
    public string? NextApproverRoleName { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? MemoFileUrl { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class BudgetRequestListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? DepartmentId { get; set; }
    public string? Status { get; set; }
    public string? UserId { get; set; }
}

public class CreateBudgetRequestDto
{
    public string DepartmentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
}

public class ApproveBudgetRequestDto
{
    public string? Comments { get; set; }
    public string? SignatureUrl { get; set; }
}

public class RejectBudgetRequestDto
{
    public string? Comments { get; set; }
    public string? SignatureUrl { get; set; }
}
