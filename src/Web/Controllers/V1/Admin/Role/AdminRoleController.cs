using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.BeemaEdgeApi.Role;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Roles;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Role;

public class AdminRoleController(IRoleService roleService) : BaseAdminApiController
{
    /// <summary>
    /// Get all system role types
    /// </summary>
    /// <returns>List of role types</returns>
    [HttpGet("types")]
    [Permission(MenuPermissionConstant.RolesView)]
    public IActionResult GetTypesAsync()
        => HandleResult(roleService.GetAllSystemRoles());

    /// <summary>
    /// Get all role names
    /// </summary>
    /// <returns>List of role names</returns>
    [HttpGet("names")]
    [Permission(MenuPermissionConstant.RolesView)]
    public async Task<IActionResult> GetNamesAsync(CancellationToken cancellationToken = default)
        => HandleResult(await roleService.GetAllRoleNamesAsync(cancellationToken));

    /// <summary>
    /// Get all roles with pagination
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <returns>Paginated list of roles</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.RolesView)]
    public async Task<IActionResult> ListAsync([FromBody] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await roleService.GetAllRolesAsync(requestModel, cancellationToken));

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="model">Role creation data</param>
    /// <returns>Success message</returns>
    [HttpPost("create")]
    [Permission(MenuPermissionConstant.RolesCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleRequestModel model, CancellationToken cancellationToken = default)
        => HandleResult(await roleService.CreateRoleAsync(model, cancellationToken));

    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role details</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.RolesView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await roleService.GetRoleByIdAsync(id, cancellationToken));

    /// <summary>
    /// Update role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="roleModel">Role update data</param>
    /// <returns>Success message</returns>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.RolesUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateRoleRequestModel roleModel, CancellationToken cancellationToken = default)
    {
        // Ensure the RoleId in the model matches the route parameter
        roleModel.RoleId = id;
        return HandleResult(await roleService.UpdateRoleAsync(roleModel, cancellationToken));
    }

    /// <summary>
    /// Delete role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.RolesDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await roleService.DeleteRoleAsync(id, cancellationToken));
}
