using System.Threading.Tasks;
using Business.BeemaEdgeApi.Permission;
using BeemaEdgeApi.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Roles;

namespace BeemaEdgeApi.Controllers.V1.Customer.Permission;

//public class CustomerMenuPermissionController : BaseApiController
//{
//    private readonly IMenuPermissionService _menuPermissionService;
//    public CustomerMenuPermissionController(IMenuPermissionService menuPermissionService) => _menuPermissionService = menuPermissionService;

//    [HttpGet]
//    [Route("GetMenu")]
//    public IActionResult GetMenu()
//    {
//        var result = _menuPermissionService.GetMenu();

//        return HandleResult(result);
//    }
//}
