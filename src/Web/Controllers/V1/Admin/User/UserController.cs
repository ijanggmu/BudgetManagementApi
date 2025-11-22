using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace BeemaEdgeApi.Controllers.V1.Admin.User;

[Route("api/v1/users")]
public class UserController(IUserService userService) : BaseAdminApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto dto)
    {
        return HandleResult(await userService.CreateAsync(dto));
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel requestModel)
    {
        return HandleResult(await userService.ListAsync(requestModel));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await userService.GetByIdAsync(id));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateUserDto dto)
    {
        return HandleResult(await userService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await userService.DeleteAsync(id));
    }
}

