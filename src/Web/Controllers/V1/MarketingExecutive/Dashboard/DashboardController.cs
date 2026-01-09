using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.AdminPortalApi.Dashboard;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.MarketingExecutive.Dashboard;

public class DashboardController(IDashboardService service) : BaseMarketingExecutiveApiController
{
    /// <summary>
    /// Get Marketing Executive dashboard
    /// </summary>
    [HttpGet("marketingexecutive")]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> GetMarketingExecutiveDashboardAsync(CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetMarketingExecutiveDashboardAsync(cancellationToken));
    }
}



