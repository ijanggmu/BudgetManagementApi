//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Role;
//using Microsoft.AspNetCore.Mvc;
//using Models.Common;
//using Models.BeemaEdgeApi.Roles;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Role;

//public class AdminRoleController : BaseApiController
//{
//    private readonly IRoleService _roleService;

//    public AdminRoleController(IRoleService roleService) => _roleService = roleService;

//    [HttpGet("GetAllRoleType")]
//    public IActionResult GetAllSystemRoles()
//    {
//        var result = _roleService.GetAllSystemRoles();
//        return HandleResult(result);
//    }

//    [HttpGet("GetAllRoleNames")]

//    public async Task<IActionResult> GetAllRoleNames()
//    {
//        var result = await _roleService.GetAllRoleNamesAsync();
//        return HandleResult(result);
//    }


//    [HttpPost("GetAllRoles")]
//    public async Task<IActionResult> GetAllRoles([FromBody] CommonPaginationRequestModel requestModel)
//    {
//        var result = await _roleService.GetAllRolesAsync(requestModel);
//        return HandleResult(result);

//    }

//    [HttpPost("CreateRole")]

//    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestModel model)
//    {
//        var result = await _roleService.CreateRoleAsync(model);

//        return HandleResult(result);

//    }


//    [HttpGet("GetRoleById/{roleId}")]

//    public async Task<IActionResult> GetRoleById(string roleId)
//    {
//        var result = await _roleService.GetRoleByIdAsync(roleId);
//        return HandleResult(result);
//    }

//    [HttpPut("UpdateRole")]

//    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequestModel roleModel)
//    {
//        var result = await _roleService.UpdateRoleAsync(roleModel);

//        return HandleResult(result);
//    }

//    [HttpDelete("DeleteRole/{roleId}")]

//    public async Task<IActionResult> DeleteRole(string roleId)
//    {
//        var result = await _roleService.DeleteRoleAsync(roleId);

//        return HandleResult(result);
//    }
//}
