using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.BeemaEdgeApi.Role;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.BeemaEdgeApi.Roles;

namespace BeemaEdgeApi.Controllers.V1.Admin.Role;

public class AdminRoleController(IRoleService roleService) : BaseAdminApiController
{
    /// <summary>
    /// Get all system role types
    /// </summary>
    /// <returns>List of role types</returns>
    [HttpGet("types")]
    public IActionResult GetTypesAsync()
        => HandleResult(roleService.GetAllSystemRoles());

    /// <summary>
    /// Get all role names
    /// </summary>
    /// <returns>List of role names</returns>
    [HttpGet("names")]
    public async Task<IActionResult> GetNamesAsync()
        => HandleResult(await roleService.GetAllRoleNamesAsync());

    /// <summary>
    /// Get all roles with pagination
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <returns>Paginated list of roles</returns>
    [HttpPost]
    public async Task<IActionResult> ListAsync([FromBody] CommonPaginationRequestModel requestModel)
        => HandleResult(await roleService.GetAllRolesAsync(requestModel));

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="model">Role creation data</param>
    /// <returns>Success message</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleRequestModel model)
        => HandleResult(await roleService.CreateRoleAsync(model));

    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
        => HandleResult(await roleService.GetRoleByIdAsync(id));

    /// <summary>
    /// Update role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="roleModel">Role update data</param>
    /// <returns>Success message</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateRoleRequestModel roleModel)
    {
        // Ensure the RoleId in the model matches the route parameter
        roleModel.RoleId = id;
        return HandleResult(await roleService.UpdateRoleAsync(roleModel));
    }

    /// <summary>
    /// Delete role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
        => HandleResult(await roleService.DeleteRoleAsync(id));
}
