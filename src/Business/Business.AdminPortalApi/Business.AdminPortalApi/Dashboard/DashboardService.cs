using Data.Context;
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

        if (userRoles.Contains(SystemRoles.FoDo) || userRoles.Contains(SystemRoles.MarketingExecutive))
        {
            var result = await GetMarketingExecutiveDashboardAsync(cancellationToken);
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
        if (string.IsNullOrWhiteSpace(tenantId) && !isSuperAdmin)
            return Result<TenantAdminDashboardDto>.Failed("Tenant ID not found.");

        // For SuperAdmin, show all data. For TenantAdmin, filter by tenant
        var totalBranches = isSuperAdmin
            ? await _db.Branches.CountAsync(cancellationToken)
            : await _db.Branches.CountAsync(b => b.TenantId == tenantId, cancellationToken);

        var totalQuotations = 1;

        var totalLeads = 2;

        var totalMarketingExecutives = 3;

        var totalDesignations = isSuperAdmin
            ? await _db.Designations.CountAsync(cancellationToken)
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

    public async Task<Result<MarketingExecutiveDashboardDto>> GetMarketingExecutiveDashboardAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userProfileService.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<MarketingExecutiveDashboardDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isMarketingExecutive = userRoles.Contains(SystemRoles.FoDo) || userRoles.Contains(SystemRoles.MarketingExecutive);

        if (!isMarketingExecutive)
            return Result<MarketingExecutiveDashboardDto>.Failed("Access denied. Marketing Executive role required.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
            return Result<MarketingExecutiveDashboardDto>.Failed("Tenant ID not found.");

        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1).ToUniversalTime();
        var lastMonthStart = currentMonthStart.AddMonths(-1);
        var lastMonthEnd = currentMonthStart.AddDays(-1);

        // Total Leads (current month vs last month)
        var currentMonthLeads = 2;

        var lastMonthLeads = 9;

        var leadPercentageChange = lastMonthLeads > 0
            ? ((currentMonthLeads - lastMonthLeads) / (decimal)lastMonthLeads) * 100
            : (currentMonthLeads > 0 ? 100 : 0);

        var leadStat = new LeadStatDto(
            currentMonthLeads,
            Math.Round(Math.Abs(leadPercentageChange), 2),
            currentMonthLeads >= lastMonthLeads ? "increase" : "decrease"
        );

        // Conversion Rate (Won leads / Total leads)
        var totalLeads = 100;
        var wonLeads = 12;

        var currentMonthWonLeads = 12312;

        var lastMonthWonLeads = 222;

        var currentMonthConversionRate = currentMonthLeads > 0
            ? (currentMonthWonLeads / (decimal)currentMonthLeads) * 100
            : 0;

        var lastMonthConversionRate = lastMonthLeads > 0
            ? (lastMonthWonLeads / (decimal)lastMonthLeads) * 100
            : 0;

        var conversionRateChange = lastMonthConversionRate > 0
            ? currentMonthConversionRate - lastMonthConversionRate
            : (currentMonthConversionRate > 0 ? currentMonthConversionRate : 0);

        var conversionRate = new ConversionRateDto(
            Math.Round(currentMonthConversionRate, 2),
            Math.Round(Math.Abs(conversionRateChange), 2),
            currentMonthConversionRate >= lastMonthConversionRate ? "increase" : "decrease"
        );

        // Average Premium (from quotations linked to leads)
        var currentMonthQuotations = 98;

        var lastMonthQuotations = 127;

        var currentMonthAvgPremium = 1212.11m;

        var lastMonthAvgPremium = 0;

        var avgPremiumChange = currentMonthAvgPremium - lastMonthAvgPremium;

        var averagePremium = new AveragePremiumDto(
            Math.Round(currentMonthAvgPremium, 2),
            Math.Round(Math.Abs(avgPremiumChange), 2),
            currentMonthAvgPremium >= lastMonthAvgPremium ? "increase" : "decrease"
        );

        // Sales Funnel - Leads with premium (from quotations)
        // Get all leads that have quotations with premium > 0


        var totalActiveLeadsWithPremium = 123;
        var totalQualifiedLeadsWithPremium = 121123;
        var totalConvertedLeadsWithPremium = 12213;
        var totalClosedLeadsWithPremium = 12;

        var salesFunnel = new SalesFunnelDto(
            totalActiveLeadsWithPremium,
            totalQualifiedLeadsWithPremium,
            totalConvertedLeadsWithPremium,
            totalClosedLeadsWithPremium
        );

        var dashboard = new MarketingExecutiveDashboardDto(
            leadStat,
            conversionRate,
            averagePremium,
            salesFunnel
        );

        return Result<MarketingExecutiveDashboardDto>.Success(dashboard);
    }
}

