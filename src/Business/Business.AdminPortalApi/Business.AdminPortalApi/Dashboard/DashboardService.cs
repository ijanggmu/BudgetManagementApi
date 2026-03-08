using Data.Context;
using Data.Entities.Tenant;
using Data.Infrastructure;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.WebApi.Dashboard;
using SharedKernel.Constant.Roles;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDataContext _db;
    private readonly IUserProfileService _userProfileService;
    private readonly ITenantResolutionService _tenantResolutionService;
    private readonly ITenantContext _tenantContext;

    public DashboardService(
        ApplicationDataContext db,
        IUserProfileService userProfileService,
        ITenantResolutionService tenantResolutionService,
        ITenantContext tenantContext)
    {
        _db = db;
        _userProfileService = userProfileService;
        _tenantResolutionService = tenantResolutionService;
        _tenantContext = tenantContext;
    }

    public async Task<Result<object>> GetDashboardByRoleAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<object>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);

        if (userRoles.Contains(SystemRoles.SuperAdmin))
        {
            var result = await GetSuperAdminDashboardAsync(cancellationToken);
            return result.IsSuccess
                ? Result<object>.Success(result.Data)
                : Result<object>.Failed(result.Error, result.ErrorCode);
        }

        if (userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin"))
        {
            var result = await GetTenantAdminDashboardAsync(cancellationToken);
            return result.IsSuccess
                ? Result<object>.Success(result.Data)
                : Result<object>.Failed(result.Error, result.ErrorCode);
        }

        if (userRoles.Contains(SystemRoles.CEO))
        {
            var result = await GetCEODashboardAsync(cancellationToken);
            return result.IsSuccess
                ? Result<object>.Success(result.Data)
                : Result<object>.Failed(result.Error, result.ErrorCode);
        }

        if (userRoles.Contains(SystemRoles.CFO))
        {
            var result = await GetCFODashboardAsync(cancellationToken);
            return result.IsSuccess
                ? Result<object>.Success(result.Data)
                : Result<object>.Failed(result.Error, result.ErrorCode);
        }

        if (userRoles.Contains(SystemRoles.HOD))
        {
            var result = await GetHODDashboardAsync(cancellationToken);
            return result.IsSuccess
                ? Result<object>.Success(result.Data)
                : Result<object>.Failed(result.Error, result.ErrorCode);
        }

        return Result<object>.Failed("Access denied. No valid role found.");
    }

    public async Task<Result<SuperAdminDashboardDto>> GetSuperAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<SuperAdminDashboardDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        if (!userRoles.Contains(SystemRoles.SuperAdmin))
            return Result<SuperAdminDashboardDto>.Failed("Access denied. SuperAdmin role required.");

        // SuperAdmin can see all data across all tenants
        var totalTenants = await _db.Tenants.CountAsync(cancellationToken);
        var totalRoles = await _db.Roles.CountAsync(cancellationToken);

        // Total leads across all tenants
        var totalLeads = 0;

        // Total admins across all tenants (including SuperAdmin's own tenant)
        var totalAdmins = await _db.Admins.CountAsync(cancellationToken);

        // Total marketing executives (Fodos) across all tenants
        var totalMarketingExecutives = 0;

        var dashboard = new SuperAdminDashboardDto(
            totalTenants,
            totalRoles,
            totalLeads,
            totalAdmins,
            totalMarketingExecutives
        );

        return Result<SuperAdminDashboardDto>.Success(dashboard);
    }

    public async Task<Result<TenantAdminDashboardDto>> GetTenantAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        var roleType = _userProfileService.GetRoleType();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<TenantAdminDashboardDto>.Failed("User not authenticated.");

        var isSuperAdmin = roleType.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roleType.Contains(SystemRoles.Admin) || roleType.Contains("TenantAdmin");

        if (!isSuperAdmin && !isAdmin)
            return Result<TenantAdminDashboardDto>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;
        var useGlobalCounts = isSuperAdmin || string.IsNullOrWhiteSpace(tenantId);

        // For SuperAdmin or demo Admin (no tenant), show all data. For TenantAdmin, filter by tenant
        var totalBranches = useGlobalCounts
            ? await _db.Branches.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Branches.CountAsync(b => b.TenantId == tenantId, cancellationToken);

        var totalQuotations = 1;

        var totalLeads = 2;

        var totalMarketingExecutives = 3;

        var totalDesignations = useGlobalCounts
            ? await _db.Designations.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Designations.CountAsync(d => d.TenantId == tenantId, cancellationToken);

        var dashboard = new TenantAdminDashboardDto(
            totalBranches,
            totalQuotations,
            totalLeads,
            totalMarketingExecutives,
            totalDesignations
        );

        return Result<TenantAdminDashboardDto>.Success(dashboard);
    }

    public async Task<Result<CEODashboardDto>> GetCEODashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<CEODashboardDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        if (!userRoles.Contains(SystemRoles.CEO))
            return Result<CEODashboardDto>.Failed("Access denied. CEO role required.");

        var tenantId = _tenantContext.TenantId;
        var pendingApprovals = string.IsNullOrEmpty(tenantId)
            ? await _db.BudgetRequests.IgnoreQueryFilters().CountAsync(r => r.Status == BudgetRequestStatus.PendingApproval, cancellationToken)
            : await _db.BudgetRequests.CountAsync(r => r.Status == BudgetRequestStatus.PendingApproval, cancellationToken);
        var totalBudgets = string.IsNullOrEmpty(tenantId)
            ? await _db.Budgets.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Budgets.CountAsync(cancellationToken);
        var totalDepts = string.IsNullOrEmpty(tenantId)
            ? await _db.Departments.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Departments.CountAsync(cancellationToken);
        var memosCount = string.IsNullOrEmpty(tenantId)
            ? await _db.Memos.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Memos.CountAsync(cancellationToken);

        var dashboard = new CEODashboardDto(pendingApprovals, totalBudgets, totalDepts, memosCount, "CEO");
        return Result<CEODashboardDto>.Success(dashboard);
    }

    public async Task<Result<CFODashboardDto>> GetCFODashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<CFODashboardDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        if (!userRoles.Contains(SystemRoles.CFO))
            return Result<CFODashboardDto>.Failed("Access denied. CFO role required.");

        var tenantId = _tenantContext.TenantId;
        var totalBudgets = string.IsNullOrEmpty(tenantId)
            ? await _db.Budgets.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Budgets.CountAsync(cancellationToken);
        var approvedCount = string.IsNullOrEmpty(tenantId)
            ? await _db.BudgetRequests.IgnoreQueryFilters().CountAsync(r => r.Status == BudgetRequestStatus.Approved, cancellationToken)
            : await _db.BudgetRequests.CountAsync(r => r.Status == BudgetRequestStatus.Approved, cancellationToken);
        var pendingCount = string.IsNullOrEmpty(tenantId)
            ? await _db.BudgetRequests.IgnoreQueryFilters().CountAsync(r => r.Status == BudgetRequestStatus.PendingApproval, cancellationToken)
            : await _db.BudgetRequests.CountAsync(r => r.Status == BudgetRequestStatus.PendingApproval, cancellationToken);
        var reportCount = string.IsNullOrEmpty(tenantId)
            ? 0
            : await _db.Budgets.IgnoreQueryFilters().CountAsync(cancellationToken);

        var dashboard = new CFODashboardDto(totalBudgets, approvedCount, pendingCount, reportCount, "CFO");
        return Result<CFODashboardDto>.Success(dashboard);
    }

    public async Task<Result<HODDashboardDto>> GetHODDashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<HODDashboardDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        if (!userRoles.Contains(SystemRoles.HOD))
            return Result<HODDashboardDto>.Failed("Access denied. HOD role required.");

        var tenantId = _tenantContext.TenantId;
        var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var departmentId = user?.DepartmentId;
        var myDeptBudgets = string.IsNullOrEmpty(departmentId)
            ? 0
            : (string.IsNullOrEmpty(tenantId)
                ? await _db.Budgets.IgnoreQueryFilters().CountAsync(b => b.DepartmentId == departmentId, cancellationToken)
                : await _db.Budgets.CountAsync(b => b.DepartmentId == departmentId, cancellationToken));
        var myMemos = string.IsNullOrEmpty(tenantId)
            ? await _db.Memos.IgnoreQueryFilters().CountAsync(m => m.CreatedBy == userId, cancellationToken)
            : await _db.Memos.CountAsync(m => m.CreatedBy == userId, cancellationToken);
        var pendingForMe = string.IsNullOrEmpty(tenantId)
            ? await _db.BudgetRequests.IgnoreQueryFilters().CountAsync(r => r.Status == BudgetRequestStatus.PendingApproval, cancellationToken)
            : await _db.BudgetRequests.CountAsync(r => r.Status == BudgetRequestStatus.PendingApproval, cancellationToken);
        var deptsCount = string.IsNullOrEmpty(tenantId)
            ? await _db.Departments.IgnoreQueryFilters().CountAsync(cancellationToken)
            : await _db.Departments.CountAsync(cancellationToken);

        var dashboard = new HODDashboardDto(myDeptBudgets, myMemos, pendingForMe, deptsCount, "HOD");
        return Result<HODDashboardDto>.Success(dashboard);
    }

    public async Task<Result<MarketingExecutiveDashboardDto>> GetMarketingExecutiveDashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<MarketingExecutiveDashboardDto>.Failed("User not authenticated.");

        // FoDo/Marketing Executive roles have been removed; this dashboard is no longer in use.
        return Result<MarketingExecutiveDashboardDto>.Failed("Access denied. Marketing Executive role has been removed.");
    }
}

