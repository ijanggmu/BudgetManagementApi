using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Budget;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.BudgetHeading;

[AdminOrSuperAdmin]
public class BudgetHeadingController(IBudgetHeadingService service) : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.BudgetView)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.BudgetView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.BudgetCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBudgetHeadingDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.BudgetUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateBudgetHeadingDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.BudgetDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));
}
