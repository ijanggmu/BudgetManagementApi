using Models.WebApi.Dashboard;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Dashboard;

public interface IDashboardService
{
    Task<Result<object>> GetDashboardByRoleAsync(CancellationToken cancellationToken = default);
    Task<Result<SuperAdminDashboardDto>> GetSuperAdminDashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<TenantAdminDashboardDto>> GetTenantAdminDashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<CEODashboardDto>> GetCEODashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<CFODashboardDto>> GetCFODashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<HODDashboardDto>> GetHODDashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<MarketingExecutiveDashboardDto>> GetMarketingExecutiveDashboardAsync(CancellationToken cancellationToken = default);
}

