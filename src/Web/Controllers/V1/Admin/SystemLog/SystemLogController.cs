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
    /// Get system access logs with pagination
    /// </summary>
    /// <param name="searchModel">Pagination and filter parameters</param>
    /// <returns>Paginated list of system access logs</returns>
    [HttpGet("access")]
    //[Permission(MenuPermissionConstant.SystemLogView)]
    public async Task<IActionResult> GetAccessLogAsync([FromBody] CommonPaginationRequestModel searchModel, CancellationToken cancellationToken = default)
        => HandleResult(await systemLogService.GetAllSystemAccessLogAsync(searchModel, cancellationToken));
}
