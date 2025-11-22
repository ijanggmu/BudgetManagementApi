//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.CustomerPassword;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using BeemaEdgeApi.Filters.AuthorizationFilters;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Customer.CustomerIdentity;
//using Models.BeemaEdgeApi.Identity;

//namespace BeemaEdgeApi.Controllers.V1.Customer.Password
//{

//    public class CustomerPasswordController(ICustomerPasswordService customerPasswordService) : BaseIndividualApiController
//    {
//        [HttpPut("Change")]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel requestModel) =>
//            HandleResult(await customerPasswordService.ChangePasswordAsync(requestModel));

//        [AllowAnonymous]
//        [SkipIndividualAuthorization]
//        [HttpPost("Forget")]
//        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestModel requestModel) =>
//            HandleResult(await customerPasswordService.ForgetPasswordAsync(requestModel));

//        [AllowAnonymous]
//        [SkipIndividualAuthorization]
//        [HttpPost("ResetPassword")]
//        public async Task<IActionResult> SetPassword([FromBody] ResetPasswordRequestModel requestModel) =>
//            HandleResult(await customerPasswordService.ResetPasswordAsync(requestModel));
//    }
//}

