using System.Threading.Tasks;
using Business.BeemaEdgeApi.Auth;
using Business.BeemaEdgeApi.Profile;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Controllers.V1.Individual.Auth
{
    [Route("api/v1/auth")]
    public class CustomerAuthController : BaseIndividualApiController
    {
        private readonly ICustomerAuthService _customerAuthService;
        private readonly ICustomerProfileService _customerProfileService;

        public CustomerAuthController(ICustomerAuthService customerAuthService, ICustomerProfileService customerProfileService)
        {
            _customerAuthService = customerAuthService;
            _customerProfileService = customerProfileService;
        }

        [AllowAnonymous]
        [SkipIndividualAuthorization]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] IndividualLoginRequestModel requestModel) => HandleResult(await _customerAuthService.LoginAsync(requestModel));

        [AllowAnonymous]
        [SkipIndividualAuthorization]
        [HttpPost("Login2FA")]
        public async Task<IActionResult> Login2FA([FromBody] Verify2FaCustomerRequestModel requestModel) => HandleResult(await _customerAuthService.Login2FaAsync(requestModel));

        [AllowAnonymous]
        [SkipIndividualAuthorization]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken() => HandleResult(await _customerAuthService.RefreshTokenAsync());

        [AllowAnonymous]
        [SkipIndividualAuthorization]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return HandleResult(_customerAuthService.Logout(HttpContext.Response));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
            => HandleResult(await _customerProfileService.GetProfileAsync());
    }
}

