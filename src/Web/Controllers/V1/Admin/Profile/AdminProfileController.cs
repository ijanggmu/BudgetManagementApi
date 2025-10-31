//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Profile;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Customer.CustomerIdentity;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Profile
//{
//    public class AdminProfileController(ICustomerProfileService customerProfileService) : BaseApiController
//    {
//        [HttpGet]
//        public async Task<IActionResult> GetProfile()
//            => HandleResult(await customerProfileService.GetProfileAsync());

//        [HttpPut("Update")]
//        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestModel requestModel)
//            => HandleResult(await customerProfileService.UpdateProfileAsync(requestModel));
//    }
//}


