using System.Threading.Tasks;
using Business.Common.PolicyCalculator;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Policy;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

public class PolicyPremiumCalculatorController(IPolicyPremiumCalculatorService policyCalculatorService) : BaseCommonApiController
{
    [HttpPost("api/v1/premiums/PolicyPremium")]
    [Permission(MenuPermissionConstant.PremiumOverviewView)]
    public async Task<IActionResult> PolicyPremium([FromBody] PremiumCalculateRequestModel policyDto)
    {
        var result = await policyCalculatorService.CalculatePolicyPremiumAsync(policyDto);
        return HandleResult(result);
    }
}

