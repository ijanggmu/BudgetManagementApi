using System.Threading.Tasks;
using Business.BeemaEdgeApi.Permission;
using BeemaEdgeApi.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Roles;
using BeemaEdgeApi.Controllers.V1.BaseController;

namespace BeemaEdgeApi.Controllers.V1.Permission;

public class AdminMenuPermissionController(IMenuPermissionService menuPermissionService) : BaseAdminApiController
{
    [HttpGet]
    [Route("GetMenu")]
    public IActionResult GetMenu()
    {
        var result = menuPermissionService.GetMenu();

        return HandleResult(result);
    }


    [HttpGet("GetAllMenuByRoleId/{roleId}")]
    public IActionResult GetAllMenuByRoleId(string roleId)
    {
        var result = menuPermissionService.GetAllMenuByRoleId(roleId);
        return HandleResult(result);
    }


    [HttpPost("ManagePermissions")]
    public async Task<IActionResult> ManagePermissions(PermissionManagementViewModel managementViewModel)
    {
        var result = await menuPermissionService.AssignRolePermissionAsync(managementViewModel);

        return HandleResult(result);
    }
}
