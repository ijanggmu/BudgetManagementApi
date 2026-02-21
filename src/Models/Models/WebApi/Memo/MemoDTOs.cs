using Models.Common;

namespace Models.BeemaEdgeApi.Memo;

public class MemoApproverDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? SignatureUrl { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class MemoResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string BudgetRequestId { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string RequestedByDepartment { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public List<MemoApproverDto> Approvers { get; set; } = new();
    public string Status { get; set; } = "Draft"; // Draft, Final
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MemoListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? DepartmentId { get; set; }
    public string? Status { get; set; }
    public string? BudgetRequestId { get; set; }
}

public class CreateMemoDto
{
    public string BudgetRequestId { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
}

public class UpdateMemoDto
{
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
}
