//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.AccountValidation;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using BeemaEdgeApi.Filters.AuthorizationFilters;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Customer.CustomerIdentity;

//namespace BeemaEdgeApi.Controllers.V1.Customer.Identity
//{
//    [AllowAnonymous]
//    [SkipIndividualAuthorization]
//    public class CustomerAccountValidationController(ICustomerAccountValidationService customerAccountValidationService) : BaseIndividualApiController
//    {
//        [HttpGet("CheckUsername")]
//        public async Task<IActionResult> CheckUsername([FromQuery] UsernameValidationRequest requestModel)
//        {
//            var exists = await customerAccountValidationService.IsUsernameTakenAsync(requestModel.Username);
//            return HandleResult(exists);
//        }

//        [HttpGet("CheckEmail")]
//        public async Task<IActionResult> CheckEmail([FromQuery] EmailValidationRequest requestModel)
//        {
//            var exists = await customerAccountValidationService.IsEmailTakenAsync(requestModel.Email);
//            return HandleResult(exists);
//        }

//        [HttpGet("CheckPhoneNumber")]
//        public async Task<IActionResult> CheckPhoneNumber([FromQuery] PhoneNumberValidationRequest requestModel)
//        {
//            var exists = await customerAccountValidationService.IsPhoneNumberTakenAsync(requestModel.PhoneNumber);
//            return HandleResult(exists);
//        }
//    }
//}

