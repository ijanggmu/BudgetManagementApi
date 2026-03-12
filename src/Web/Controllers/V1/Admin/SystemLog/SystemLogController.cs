using System.Threading;
using System.Threading.Tasks;
using AdminPortalApi.Controllers.V1.SystemLog;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.SystemLog;

public class SystemLogController(ISystemLogService systemLogService) : BaseAdminApiController
{
    /// <summary>
    /// Get system access logs with pagination (Admin/SuperAdmin only; showable logs with friendly names).
    /// </summary>
    [HttpPost("access")]
    [Permission(MenuPermissionConstant.SystemLogView)]
    public async Task<IActionResult> GetAccessLogAsync([FromBody] CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default)
        => HandleResult(await systemLogService.GetAllSystemAccessLogAsync(searchModel, cancellationToken));

    /// <summary>
    /// Get recent activity for dashboard. Scope: Individual = own, Admin = tenant, SuperAdmin = all. Showable only, friendly names.
    /// </summary>
    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivityAsync([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        => HandleResult(await systemLogService.GetRecentActivityAsync(limit, cancellationToken));

    /// <summary>
    /// Get activity log (paginated) for Activity Log page. Same scope as recent-activity.
    /// </summary>
    [HttpPost("activity")]
    public async Task<IActionResult> GetActivityLogAsync([FromBody] CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default)
        => HandleResult(await systemLogService.GetActivityLogAsync(searchModel, cancellationToken));
}
