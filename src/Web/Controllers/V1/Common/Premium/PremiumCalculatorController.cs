using System.Threading.Tasks;
using Business.Common.PolicyCalculator;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Policy;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

public class PremiumCalculatorController(IPolicyCalculatorService policyCalculatorService) : BaseCommonApiController
{
    [HttpPost("api/v1/premiums/calculate")]
    [Permission(MenuPermissionConstant.PremiumOverviewView)]
    public async Task<IActionResult> CalculatePremium([FromBody] PremiumCalculateRequestModel policyDto)
    {
        var result = await policyCalculatorService.CalculatePremiumAsync(policyDto);
        return HandleResult(result);
    }
}

