using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.AdminPortalApi.Dashboard;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Dashboard;

[Route("api/v1/admin/dashboard")]
public class AdminDashboardController(IDashboardService service) : BaseAdminApiController
{
    /// <summary>
    /// Get dashboard data based on user role
    /// Returns SuperAdmin, TenantAdmin, or MarketingExecutive dashboard based on authenticated user's role
    /// </summary>
    [HttpGet]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetDashboardByRoleAsync(cancellationToken));
    }

    /// <summary>
    /// Get SuperAdmin dashboard
    /// </summary>
    [HttpGet("superadmin")]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> GetSuperAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetSuperAdminDashboardAsync(cancellationToken));
    }

    /// <summary>
    /// Get TenantAdmin dashboard
    /// </summary>
    [HttpGet("tenantadmin")]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> GetTenantAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetTenantAdminDashboardAsync(cancellationToken));
    }


}

