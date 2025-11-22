using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.AdminPortalApi.Profile;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;

namespace BeemaEdgeApi.Controllers.V1.Admin.Profile
{
    public class AdminProfileController(IAdminProfileService adminProfileService) : BaseAdminApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetProfile()
            => HandleResult(await adminProfileService.GetProfileAsync());

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestModel requestModel)
            => HandleResult(await adminProfileService.UpdateProfileAsync(requestModel));
    }
}


