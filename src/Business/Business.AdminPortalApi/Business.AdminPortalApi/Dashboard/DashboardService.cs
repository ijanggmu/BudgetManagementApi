using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.FodoEntity;
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
        var totalLeads = await _db.Leads.CountAsync(cancellationToken);

        // Total admins across all tenants (including SuperAdmin's own tenant)
        var totalAdmins = await _db.Admins.CountAsync(cancellationToken);

        // Total marketing executives (Fodos) across all tenants
        var totalMarketingExecutives = await _db.Fodos.CountAsync(cancellationToken);

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
        if (string.IsNullOrWhiteSpace(userId))
            return Result<TenantAdminDashboardDto>.Failed("User not authenticated.");

        var userRoles = await _tenantResolutionService.GetUserRolesAsync(userId);
        var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = userRoles.Contains(SystemRoles.Admin) || userRoles.Contains("TenantAdmin");

        if (!isSuperAdmin && !isAdmin)
            return Result<TenantAdminDashboardDto>.Failed("Access denied. Admin role required.");

        var tenantId = _tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId) && !isSuperAdmin)
            return Result<TenantAdminDashboardDto>.Failed("Tenant ID not found.");

        // For SuperAdmin, show all data. For TenantAdmin, filter by tenant
        var totalBranches = isSuperAdmin
            ? await _db.Branches.CountAsync(cancellationToken)
            : await _db.Branches.CountAsync(b => b.TenantId == tenantId, cancellationToken);

        var totalQuotations = isSuperAdmin
            ? await _db.Quotations.CountAsync(cancellationToken)
            : await _db.Quotations.CountAsync(q => q.TenantId == tenantId, cancellationToken);

        var totalLeads = isSuperAdmin
            ? await _db.Leads.CountAsync(cancellationToken)
            : await _db.Leads.CountAsync(l => l.TenantId == tenantId, cancellationToken);

        var totalMarketingExecutives = isSuperAdmin
            ? await _db.Fodos.CountAsync(cancellationToken)
            : await _db.Fodos.CountAsync(f => f.TenantId == tenantId, cancellationToken);

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
        var currentMonthLeads = await _db.Leads
            .CountAsync(l => l.TenantId == tenantId &&
                           l.OwnerUserId == userId &&
                           l.CreatedOn >= currentMonthStart, cancellationToken);

        var lastMonthLeads = await _db.Leads
            .CountAsync(l => l.TenantId == tenantId &&
                           l.OwnerUserId == userId &&
                           l.CreatedOn >= lastMonthStart &&
                           l.CreatedOn <= lastMonthEnd, cancellationToken);

        var leadPercentageChange = lastMonthLeads > 0
            ? ((currentMonthLeads - lastMonthLeads) / (decimal)lastMonthLeads) * 100
            : (currentMonthLeads > 0 ? 100 : 0);

        var leadStat = new LeadStatDto(
            currentMonthLeads,
            Math.Round(Math.Abs(leadPercentageChange), 2),
            currentMonthLeads >= lastMonthLeads ? "increase" : "decrease"
        );

        // Conversion Rate (Won leads / Total leads)
        var totalLeads = await _db.Leads
            .CountAsync(l => l.TenantId == tenantId && l.OwnerUserId == userId, cancellationToken);

        var wonLeads = await _db.Leads
            .CountAsync(l => l.TenantId == tenantId &&
                           l.OwnerUserId == userId &&
                           l.Status == LeadStatus.Won, cancellationToken);

        var currentMonthWonLeads = await _db.Leads
            .CountAsync(l => l.TenantId == tenantId &&
                           l.OwnerUserId == userId &&
                           l.Status == LeadStatus.Won &&
                           l.CreatedOn >= currentMonthStart, cancellationToken);

        var lastMonthWonLeads = await _db.Leads
            .CountAsync(l => l.TenantId == tenantId &&
                           l.OwnerUserId == userId &&
                           l.Status == LeadStatus.Won &&
                           l.CreatedOn >= lastMonthStart &&
                           l.CreatedOn <= lastMonthEnd, cancellationToken);

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
        var currentMonthQuotations = await _db.Quotations
            .Where(q => q.TenantId == tenantId &&
                       q.CreatedOn >= currentMonthStart)
            .Join(_db.Leads.Where(l => l.TenantId == tenantId && l.OwnerUserId == userId),
                q => q.ProspectId,
                l => l.ProspectId,
                (q, l) => q.TotalPremium ?? 0)
            .ToListAsync(cancellationToken);

        var lastMonthQuotations = await _db.Quotations
            .Where(q => q.TenantId == tenantId &&
                       q.CreatedOn >= lastMonthStart &&
                       q.CreatedOn <= lastMonthEnd)
            .Join(_db.Leads.Where(l => l.TenantId == tenantId && l.OwnerUserId == userId),
                q => q.ProspectId,
                l => l.ProspectId,
                (q, l) => q.TotalPremium ?? 0)
            .ToListAsync(cancellationToken);

        var currentMonthAvgPremium = currentMonthQuotations.Any()
            ? currentMonthQuotations.Average()
            : 0;

        var lastMonthAvgPremium = lastMonthQuotations.Any()
            ? lastMonthQuotations.Average()
            : 0;

        var avgPremiumChange = currentMonthAvgPremium - lastMonthAvgPremium;

        var averagePremium = new AveragePremiumDto(
            Math.Round(currentMonthAvgPremium, 2),
            Math.Round(Math.Abs(avgPremiumChange), 2),
            currentMonthAvgPremium >= lastMonthAvgPremium ? "increase" : "decrease"
        );

        // Sales Funnel - Leads with premium (from quotations)
        // Get all leads that have quotations with premium > 0
        var leadsWithQuotationsQuery = from lead in _db.Leads
                                       join quotation in _db.Quotations on lead.ProspectId equals quotation.ProspectId
                                       where lead.TenantId == tenantId
                                          && lead.OwnerUserId == userId
                                          && quotation.TenantId == tenantId
                                          && quotation.TotalPremium.HasValue
                                          && quotation.TotalPremium > 0
                                       select lead;

        var leadsWithQuotations = await leadsWithQuotationsQuery
            .Distinct()
            .ToListAsync(cancellationToken);

        var totalActiveLeadsWithPremium = leadsWithQuotations.Count(l => l.Status == LeadStatus.New || l.Status == LeadStatus.Qualified || l.Status == LeadStatus.Contacted);
        var totalQualifiedLeadsWithPremium = leadsWithQuotations.Count(l => l.Status == LeadStatus.Qualified);
        var totalConvertedLeadsWithPremium = leadsWithQuotations.Count(l => l.Status == LeadStatus.Won);
        var totalClosedLeadsWithPremium = leadsWithQuotations.Count(l => l.Status == LeadStatus.Won || l.Status == LeadStatus.Lost);

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

