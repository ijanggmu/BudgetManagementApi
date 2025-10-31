using System.Threading;
using System.Threading.Tasks;
using Business.Common.Policy;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace BeemaEdgeApi.Controllers.V1.Common.Manufacture;

public class ManufactureController(IPolicyService policyService) : BaseCommonApiController
{
    [HttpGet("GetAllManufactureByClassName")]
    public async Task<IActionResult> GetAllManufacture(string className)
    {
        var result = await policyService.GetAllManufactureByClassName(className);
        return HandleResult(result);
    }

    [HttpPost("GetAllManufacture")]
    public async Task<IActionResult> GetAllManufacture(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken)
    {
        var result = await policyService.GetAllManufacture(requestModel, cancellationToken);
        return HandleResult(result);
    }
}
