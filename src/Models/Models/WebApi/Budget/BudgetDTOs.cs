using Models.Common;

namespace Models.BeemaEdgeApi.Budget;

// Nepali Fiscal Year
public class NepaliFiscalYearResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string? Description { get; set; }
}

public class NepaliFiscalYearListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
}

public class CreateNepaliFiscalYearDto
{
    public string Code { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string? Description { get; set; }
}

public class UpdateNepaliFiscalYearDto
{
    public string? Code { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public string? Description { get; set; }
}

public class BudgetResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string? BudgetHeadingId { get; set; }
    public string? BudgetHeadingName { get; set; }
    public string? BudgetSubheadingId { get; set; }
    public string? BudgetSubheadingName { get; set; }
    public string? FiscalYearId { get; set; }
    public string? FiscalYearCode { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    public decimal Unit { get; set; }
    public decimal UnitAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsLocked { get; set; }
    public int Version { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class BudgetListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? DepartmentId { get; set; }
    public string? BudgetHeadingId { get; set; }
    public string? BudgetSubheadingId { get; set; }
    public string? FiscalYearId { get; set; }
    public int? Year { get; set; }
    public int? Quarter { get; set; }
}

public class CreateBudgetDto
{
    public string DepartmentId { get; set; } = string.Empty;
    public string? BudgetHeadingId { get; set; }
    public string? BudgetSubheadingId { get; set; }
    public string? FiscalYearId { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    /// <summary>Quantity/units. TotalAmount = Unit * UnitAmount.</summary>
    public decimal Unit { get; set; } = 1;
    /// <summary>Amount per unit. TotalAmount = Unit * UnitAmount.</summary>
    public decimal UnitAmount { get; set; }
}

public class UpdateBudgetDto
{
    public string? DepartmentId { get; set; }
    public string? BudgetHeadingId { get; set; }
    public string? BudgetSubheadingId { get; set; }
    public string? FiscalYearId { get; set; }
    public int? Year { get; set; }
    public int? Quarter { get; set; }
    public decimal? Unit { get; set; }
    public decimal? UnitAmount { get; set; }
}

public class SetBudgetLockDto
{
    public bool IsLocked { get; set; }
}

// Budget Heading (list supports pagination)
public class BudgetHeadingListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
}

// Budget Subheading (list supports pagination)
public class BudgetSubheadingListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? BudgetHeadingId { get; set; }
}

// Budget Heading
public class BudgetHeadingResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateBudgetHeadingDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateBudgetHeadingDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
}

// Budget Subheading
public class BudgetSubheadingResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string BudgetHeadingId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateBudgetSubheadingDto
{
    public string BudgetHeadingId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateBudgetSubheadingDto
{
    public string? BudgetHeadingId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
}
