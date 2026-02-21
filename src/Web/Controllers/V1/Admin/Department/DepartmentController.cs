using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Department;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Department;

[AdminOrSuperAdmin]
public class DepartmentController(IDepartmentService service) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.DepartmentView)]
    public async Task<IActionResult> ListAsync([FromBody] DepartmentListRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(requestModel ?? new DepartmentListRequestModel(), cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.DepartmentView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.DepartmentCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDepartmentDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.DepartmentUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.DepartmentDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));
}
