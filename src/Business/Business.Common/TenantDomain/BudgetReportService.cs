using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.BudgetReport;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BudgetReportService(ApplicationDataContext db, IUserProfileService userProfileService) : IBudgetReportService
{
    public async Task<Result<BudgetReportResponseDto>> GetReportAsync(BudgetReportRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        IQueryable<Data.Entities.Tenant.Budget> budgetQuery = db.Budgets.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            budgetQuery = budgetQuery.IgnoreQueryFilters().Where(b => b.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            budgetQuery = budgetQuery.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
            budgetQuery = budgetQuery.Where(b => b.DepartmentId == requestModel.DepartmentId);
        if (!string.IsNullOrEmpty(requestModel.FiscalYearId))
            budgetQuery = budgetQuery.Where(b => b.FiscalYearId == requestModel.FiscalYearId);
        else
        {
            if (requestModel.Year.HasValue)
                budgetQuery = budgetQuery.Where(b => b.Year == requestModel.Year.Value);
            if (requestModel.Quarter.HasValue)
                budgetQuery = budgetQuery.Where(b => b.Quarter == requestModel.Quarter.Value);
        }

        var budgetSums = await budgetQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalBudget = g.Sum(b => b.TotalAmount),
                TotalAllocated = g.Sum(b => b.AllocatedAmount),
                TotalRemaining = g.Sum(b => b.RemainingAmount)
            })
            .FirstOrDefaultAsync(cancellationToken);

        IQueryable<BudgetRequest> requestQuery = db.BudgetRequests.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            requestQuery = requestQuery.IgnoreQueryFilters().Where(r => r.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            requestQuery = requestQuery.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
            requestQuery = requestQuery.Where(r => r.DepartmentId == requestModel.DepartmentId);
        if (!string.IsNullOrEmpty(requestModel.FiscalYearId))
        {
            var fy = await db.NepaliFiscalYears
                .AsQueryable()
                .Where(f => f.Id == requestModel.FiscalYearId)
                .Select(f => new { f.StartYear, f.EndYear })
                .FirstOrDefaultAsync(cancellationToken);
            if (fy != null)
                requestQuery = requestQuery.Where(r => r.RequestedDate.Year >= fy.StartYear && r.RequestedDate.Year <= fy.EndYear);
        }
        else if (requestModel.Year.HasValue)
            requestQuery = requestQuery.Where(r => r.RequestedDate.Year == requestModel.Year.Value);

        var requestCounts = await requestQuery
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalBudget = budgetSums?.TotalBudget ?? 0;
        var totalAllocated = budgetSums?.TotalAllocated ?? 0;
        var totalRemaining = budgetSums?.TotalRemaining ?? 0;
        var utilizationRate = totalBudget > 0 ? (double)(totalAllocated / totalBudget) * 100.0 : 0;

        var approved = requestCounts.Where(c => c.Status == BudgetRequestStatus.Approved).Sum(c => c.Count);
        var rejected = requestCounts.Where(c => c.Status == BudgetRequestStatus.Rejected).Sum(c => c.Count);
        var pending = requestCounts.Where(c => c.Status == BudgetRequestStatus.PendingApproval).Sum(c => c.Count);
        var totalRequests = approved + rejected + pending;

        var dto = new BudgetReportResponseDto
        {
            TotalBudget = totalBudget,
            TotalAllocated = totalAllocated,
            TotalRemaining = totalRemaining,
            UtilizationRatePercent = Math.Round(utilizationRate, 2),
            TotalRequests = totalRequests,
            ApprovedRequests = approved,
            RejectedRequests = rejected,
            PendingRequests = pending
        };
        return Result<BudgetReportResponseDto>.Success(dto);
    }
}
