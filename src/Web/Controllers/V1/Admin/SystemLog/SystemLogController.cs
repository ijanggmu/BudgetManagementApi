using System.Threading.Tasks;
using AdminPortalApi.Controllers.V1.SystemLog;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace BeemaEdgeApi.Controllers.V1.Admin.SystemLog;

public class SystemLogController : BaseAdminApiController
{
    private readonly ISystemLogService _systemLogService;
    public SystemLogController(ISystemLogService systemLogService)
    {
        _systemLogService = systemLogService;
    }

    [HttpPost("GetSystemAccessLog")]
    public async Task<IActionResult> GetSystemAccessLog([FromBody] CommonPaginationRequestModel searchModel)
    {
        var result = await _systemLogService.GetAllSystemAccessLogAsync(searchModel);
        return HandleResult(result);
    }

}
