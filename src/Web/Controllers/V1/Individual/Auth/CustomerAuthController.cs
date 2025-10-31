using System.Threading.Tasks;
using Business.BeemaEdgeApi.Auth;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Controllers.V1.Individual.Auth
{
    [AllowAnonymous]
    [SkipIndividualAuthorization]
    public class CustomerAuthController(ICustomerAuthService customerAuthService) : BaseIndividualApiController
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] IndividualLoginRequestModel requestModel) => HandleResult(await customerAuthService.LoginAsync(requestModel));

        [HttpPost("Login2FA")]
        public async Task<IActionResult> Login2FA([FromBody] Verify2FaCustomerRequestModel requestModel) => HandleResult(await customerAuthService.Login2FaAsync(requestModel));

        [HttpGet("Refresh")]
        public async Task<IActionResult> RefreshToken() => HandleResult(await customerAuthService.RefreshTokenAsync());

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return HandleResult(customerAuthService.Logout(HttpContext.Response));
        }
    }
}

