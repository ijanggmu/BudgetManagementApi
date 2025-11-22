//using System.Threading.Tasks;
//using Business.Common.Otp;
//using Business.BeemaEdgeApi.Registration;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using BeemaEdgeApi.Filters.AuthorizationFilters;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Identity;

//namespace BeemaEdgeApi.Controllers.V1.Individual.Registration
//{
//    [AllowAnonymous]
//    [SkipIndividualAuthorization]
//    public class CustomerRegistrationController(ICustomerRegistrationService customerRegistrationService) : BaseIndividualApiController
//    {
//        [HttpPost("Register")]
//        public async Task<IActionResult> Register([FromBody] RegisterCustomerRequestModel requestModel)
//            => HandleResult(await customerRegistrationService.RegisterAsync(requestModel));

//        [HttpPost("VerifyOtp")]
//        public async Task<IActionResult> VerifyCustomerOtp([FromBody] VerifyCustomerOtpRequestModel requestModel)
//            => HandleResult(await customerRegistrationService.VerifyCustomerOtpAsync(requestModel));
//    }
//}


