//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.AccountValidation;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Customer.CustomerIdentity;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Identity
//{
//    [AllowAnonymous]
//    public class AdminAccountValidationController(ICustomerAccountValidationService customerAccountValidationService) : BaseApiController
//    {
//        [HttpGet("CheckUsername")]
//        public async Task<IActionResult> CheckUsername([FromQuery] UsernameValidationRequest requestModel)
//        {
//            var exists = await customerAccountValidationService.IsUsernameTakenAsync(requestModel.Username);
//            return Ok(new { exists });
//        }

//        [HttpGet("CheckEmail")]
//        public async Task<IActionResult> CheckEmail([FromQuery] EmailValidationRequest requestModel)
//        {
//            var exists = await customerAccountValidationService.IsEmailTakenAsync(requestModel.Email);
//            return Ok(new { exists });
//        }

//        [HttpGet("CheckPhoneNumber")]
//        public async Task<IActionResult> CheckPhoneNumber([FromQuery] PhoneNumberValidationRequest requestModel)
//        {
//            var exists = await customerAccountValidationService.IsPhoneNumberTakenAsync(requestModel.PhoneNumber);
//            return Ok(new { exists });
//        }
//    }
//}

