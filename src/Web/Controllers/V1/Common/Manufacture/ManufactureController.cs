using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.Policy;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Manufacture;

public class ManufactureController(IPolicyService policyService) : BaseCommonApiController
{
    [HttpGet("GetAllManufactureByClassName")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetAllManufacture(string className)
    {
        var result = await policyService.GetAllManufactureByClassName(className);
        return HandleResult(result);
    }

    [HttpPost("GetAllManufacture")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetAllManufacture(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken)
    {
        var result = await policyService.GetAllManufacture(requestModel, cancellationToken);
        return HandleResult(result);
    }
}
