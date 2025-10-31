//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.CustomerPassword;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Customer.CustomerIdentity;
//using Models.BeemaEdgeApi.Identity;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Password
//{
//    public class AdminPasswordController(ICustomerPasswordService customerPasswordService) : BaseApiController
//    {
//        [HttpPut("Change")]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel requestModel) => HandleResult(await customerPasswordService.ChangePasswordAsync(requestModel));

//        [AllowAnonymous]
//        [HttpPost("Forget")]
//        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestModel requestModel) => HandleResult(await customerPasswordService.ForgetPasswordAsync(requestModel));

//        [HttpPost("Set")]
//        public async Task<IActionResult> SetPassword([FromBody] ChangePasswordRequestModel requestModel) => HandleResult(await customerPasswordService.SetPasswordAsync(requestModel));
//    }
//}

