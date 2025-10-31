using System.Threading.Tasks;
using Business.BeemaEdgeApi.Kyc;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;

namespace BeemaEdgeApi.Controllers.V1.Customer.Kyc
{
    public class CustomerKycController(ICustomerKycService customerProfileService) : BaseIndividualApiController
    {
        [HttpPost("SetKyc")]
        public async Task<IActionResult> SetKyc(SetKycRequestModel requestModel)
            => HandleResult(await customerProfileService.SetKycAsync(requestModel));

        [HttpGet("GetKyc")]
        public async Task<IActionResult> GetKyc()
            => HandleResult(await customerProfileService.GetKycAsync());
    }
}


