using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.AdminPortalApi.Profile;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;

namespace BeemaEdgeApi.Controllers.V1.Admin.Profile;

public class AdminProfileController(IAdminProfileService adminProfileService) : BaseAdminApiController
{
    /// <summary>
    /// Get current admin user profile
    /// </summary>
    /// <returns>Admin profile information</returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync()
        => HandleResult(await adminProfileService.GetProfileAsync());

    /// <summary>
    /// Update admin user profile
    /// </summary>
    /// <param name="requestModel">Profile update data</param>
    /// <returns>Success message</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateProfileRequestModel requestModel)
        => HandleResult(await adminProfileService.UpdateProfileAsync(requestModel));
}


