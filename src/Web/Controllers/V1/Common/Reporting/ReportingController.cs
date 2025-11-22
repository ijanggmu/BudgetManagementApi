using System.Threading.Tasks;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.V1.Common.Reporting;

[ApiController]
[Route("api/v1/reporting")]
[Authorize]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _service;

    public ReportingController(IReportingService service)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _service.GetDashboardStatsAsync(userId);
        return Ok(result);
    }
}
