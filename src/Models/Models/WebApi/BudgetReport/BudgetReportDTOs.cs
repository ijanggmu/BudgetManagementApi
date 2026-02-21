namespace Models.BeemaEdgeApi.BudgetReport;

public class BudgetReportRequestModel
{
    public string? TenantId { get; set; }
    public string? DepartmentId { get; set; }
    public int? Year { get; set; }
    public int? Quarter { get; set; }
}

public class BudgetReportResponseDto
{
    public decimal TotalBudget { get; set; }
    public decimal TotalAllocated { get; set; }
    public decimal TotalRemaining { get; set; }
    public double UtilizationRatePercent { get; set; }
    public int TotalRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int PendingRequests { get; set; }
}
