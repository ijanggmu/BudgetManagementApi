using System.Threading.Tasks;
using Business.Common.PolicyCalculator;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Policy;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

public class PremiumCalculatorController(IPolicyCalculatorService policyCalculatorService) : BaseCommonApiController
{
    [HttpPost("CalculatePremium")]
    public async Task<IActionResult> CalculatePremium([FromBody] PremiumCalculateRequestModel policyDto)
    {
        var result = await policyCalculatorService.CalculatePremiumAsync(policyDto);
        return HandleResult(result);
    }
}

