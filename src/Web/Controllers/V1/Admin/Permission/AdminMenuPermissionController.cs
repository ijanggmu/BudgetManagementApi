//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Permission;
//using BeemaEdgeApi.Controllers.V1;
//using Microsoft.AspNetCore.Mvc;
//using Models.BeemaEdgeApi.Roles;

//namespace BeemaEdgeApi.Controllers.V1.Permission;

//public class AdminMenuPermissionController : BaseApiController
//{
//    private readonly IMenuPermissionService _menuPermissionService;
//    public AdminMenuPermissionController(IMenuPermissionService menuPermissionService) => _menuPermissionService = menuPermissionService;

//    [HttpGet]
//    [Route("GetMenu")]
//    public IActionResult GetMenu()
//    {
//        var result = _menuPermissionService.GetMenu();

//        return HandleResult(result);
//    }


//    [HttpGet("GetAllMenuByRoleId/{roleId}")]
//    public IActionResult GetAllMenuByRoleId(string roleId)
//    {
//        var result = _menuPermissionService.GetAllMenuByRoleId(roleId);
//        return HandleResult(result);
//    }


//    [HttpPost("ManagePermissions")]
//    public async Task<IActionResult> ManagePermissions(PermissionManagementViewModel managementViewModel)
//    {
//        var result = await _menuPermissionService.AssignRolePermissionAsync(managementViewModel);

//        return HandleResult(result);
//    }
//}
