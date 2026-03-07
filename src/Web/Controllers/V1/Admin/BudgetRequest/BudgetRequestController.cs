using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.BudgetRequest;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.BudgetRequest;

[AdminOrSuperAdmin]
public class BudgetRequestController(IBudgetRequestService service) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.BudgetRequestView)]
    public async Task<IActionResult> ListAsync([FromBody] BudgetRequestListRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(requestModel ?? new BudgetRequestListRequestModel(), cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.BudgetRequestView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.BudgetRequestCreate)]
    [Permission(MenuPermissionConstant.BudgetRequestCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBudgetRequestDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    [HttpPost("{id}/approve")]
    [Permission(MenuPermissionConstant.BudgetRequestApprove)]
    public async Task<IActionResult> ApproveAsync(string id, [FromBody] ApproveBudgetRequestDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.ApproveAsync(id, dto ?? new ApproveBudgetRequestDto(), cancellationToken));

    [HttpPost("{id}/reject")]
    [Permission(MenuPermissionConstant.BudgetRequestReject)]
    public async Task<IActionResult> RejectAsync(string id, [FromBody] RejectBudgetRequestDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.RejectAsync(id, dto ?? new RejectBudgetRequestDto(), cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.BudgetRequestView)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));

    [HttpPost("export")]
    [Permission(MenuPermissionConstant.BudgetReportExport)]
    public async Task<IActionResult> ExportAsync([FromBody] BudgetRequestListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var result = await service.ExportAsync(requestModel ?? new BudgetRequestListRequestModel(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(SharedKernel.Operation.ErrorApiResponse.WrapError(result.Error, result.ErrorCode));
        return File(result.Data, "text/csv", "budget-requests.csv");
    }
}
