using Models.Common;

namespace Models.BeemaEdgeApi.Budget;

public class BudgetResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class BudgetListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? DepartmentId { get; set; }
    public int? Year { get; set; }
    public int? Quarter { get; set; }
}

public class CreateBudgetDto
{
    public string DepartmentId { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Quarter { get; set; }
    public decimal TotalAmount { get; set; }
}

public class UpdateBudgetDto
{
    public string? DepartmentId { get; set; }
    public int? Year { get; set; }
    public int? Quarter { get; set; }
    public decimal? TotalAmount { get; set; }
}
