using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.AdminPortalApi.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Profile;

public class AdminProfileController(IAdminProfileService adminProfileService) : BaseAdminApiController
{
    /// <summary>
    /// Get current admin user profile
    /// </summary>
    /// <returns>Admin profile information</returns>
    ///
    [HttpGet]
    //[Permission(MenuPermissionConstant.ProfileView)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        => HandleResult(await adminProfileService.GetProfileAsync(cancellationToken));

    /// <summary>
    /// Update admin user profile
    /// </summary>
    /// <param name="requestModel">Profile update data</param>
    /// <returns>Success message</returns>
    [HttpPut]
    [Permission(MenuPermissionConstant.ProfileUpdate)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateProfileRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await adminProfileService.UpdateProfileAsync(requestModel, cancellationToken));
}


