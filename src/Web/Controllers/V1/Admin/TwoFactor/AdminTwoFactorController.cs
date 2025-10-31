//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.TwoFactor;
//using Microsoft.AspNetCore.Mvc;

//namespace BeemaEdgeApi.Controllers.V1.Admin.TwoFactor
//{
//    public class AdminTwoFactorController(ICustomerTwoFactorService customerTwoFactorService) : BaseApiController
//    {
//        [HttpGet("Setup")]
//        public async Task<IActionResult> Set2Fa() => HandleResult(await customerTwoFactorService.Set2FaAsync());

//        [HttpPut("Disable")]
//        public async Task<IActionResult> Disable2Fa() => HandleResult(await customerTwoFactorService.Disable2FaAsync());

//        [HttpGet("Verify/{code}")]
//        public async Task<IActionResult> Verify2Fa(string code) => HandleResult(await customerTwoFactorService.ValidateTotpCodeAsync(code));

//        [HttpGet("BackupCodes")]
//        public async Task<IActionResult> GetAll2FaBackUpCodes() => HandleResult(await customerTwoFactorService.GetAll2FaBackUpCodesAsync());
//    }
//}
