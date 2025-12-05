using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.BeemaEdgeApi.Permission;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Roles;

namespace BeemaEdgeApi.Controllers.V1.Admin.Permission;

public class AdminMenuPermissionController(IMenuPermissionService menuPermissionService) : BaseAdminApiController
{
    /// <summary>
    /// Get all menu items with permissions
    /// </summary>
    /// <returns>Menu structure with permissions</returns>
    [HttpGet("GetMenu")]
    public IActionResult GetMenuAsync()
        => HandleResult(menuPermissionService.GetMenu());

    /// <summary>
    /// Get menu permissions by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Menu permissions for the specified role</returns>
    [HttpGet("role/{roleId}")]
    public IActionResult GetByRoleIdAsync(string roleId)
        => HandleResult(menuPermissionService.GetAllMenuByRoleId(roleId));

    /// <summary>
    /// Assign permissions to a role
    /// </summary>
    /// <param name="managementViewModel">Permission management data</param>
    /// <returns>Success message</returns>
    [HttpPost]
    public async Task<IActionResult> AssignAsync([FromBody] PermissionManagementViewModel managementViewModel)
        => HandleResult(await menuPermissionService.AssignRolePermissionAsync(managementViewModel));
}
