using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace Web.Controllers.V1.Common.Reporting;

[ApiController]
[Route("api/v1/reporting")]
[Authorize]
public class ReportingController : BaseCommonApiController
{
    private readonly IReportingService _service;

    public ReportingController(IReportingService service)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    [Permission(MenuPermissionConstant.ReportingView)]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _service.GetDashboardStatsAsync(userId);
        return Ok(result);
    }
}
