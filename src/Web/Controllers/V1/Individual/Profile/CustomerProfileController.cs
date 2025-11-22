//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Profile;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Customer.CustomerIdentity;

//namespace BeemaEdgeApi.Controllers.V1.Customer.Profile
//{
//    public class CustomerProfileController(ICustomerProfileService customerProfileService) : BaseIndividualApiController
//    {
//        [HttpGet]
//        public async Task<IActionResult> GetProfile()
//            => HandleResult(await customerProfileService.GetProfileAsync());

//        [HttpPut("Update")]
//        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestModel requestModel)
//            => HandleResult(await customerProfileService.UpdateProfileAsync(requestModel));
//    }
//}


