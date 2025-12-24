using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace Web.Controllers.V1.Common.Renewal;

[ApiController]
[Route("api/v1/renewals")]
[Authorize]
public class RenewalController : BaseCommonApiController
{
    private readonly IRenewalService _service;

    public RenewalController(IRenewalService service)
    {
        _service = service;
    }

    [HttpPost("trigger-reminders")]
    [Authorize(Policy = "AdminOnly")] // Example policy
    [Permission(MenuPermissionConstant.OperationsView)]
    public async Task<IActionResult> TriggerReminders()
    {
        await _service.CheckAndSendRemindersAsync();
        return Ok("Reminders triggered");
    }

    [HttpGet("my-reminders")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetMyReminders()
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _service.GetRemindersForUserAsync(userId);
        return Ok(result);
    }
}
