using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Budget;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.BudgetSubheading;

[AdminOrSuperAdmin]
public class BudgetSubheadingController(IBudgetSubheadingService service) : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.BudgetView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? budgetHeadingId, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(budgetHeadingId, cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.BudgetView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.BudgetCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBudgetSubheadingDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.BudgetUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateBudgetSubheadingDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.BudgetDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));
}
