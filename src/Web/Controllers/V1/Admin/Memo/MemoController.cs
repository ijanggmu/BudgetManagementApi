using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Memo;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Memo;

[BmsPortalUser]
public class MemoController(IMemoService service, IBudgetRequestService budgetRequestService) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.MemoView)]
    public async Task<IActionResult> ListAsync([FromBody] MemoListRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(requestModel ?? new MemoListRequestModel(), cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.MemoView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpGet("budget-request/{budgetRequestId}")]
    [Permission(MenuPermissionConstant.MemoView)]
    public async Task<IActionResult> GetByBudgetRequestAsync(string budgetRequestId, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByBudgetRequestIdAsync(budgetRequestId, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.MemoCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMemoDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    /// <summary>Single flow: create a budget request and its memo in one call (HOD creates a memo requesting an item).</summary>
    [HttpPost("create-request")]
    [Permission(MenuPermissionConstant.MemoCreate)]
    public async Task<IActionResult> CreateRequestAsync([FromBody] CreateRequestMemoDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await budgetRequestService.CreateRequestWithMemoAsync(dto ?? new CreateRequestMemoDto(), cancellationToken));

    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.MemoUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateMemoDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.UpdateAsync(id, dto ?? new UpdateMemoDto(), cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.MemoDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));

    [HttpGet("{id}/generate-pdf")]
    [Permission(MenuPermissionConstant.MemoGeneratePdf)]
    public async Task<IActionResult> GeneratePdfAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await service.GeneratePdfAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(SharedKernel.Operation.ErrorApiResponse.WrapError(result.Error, result.ErrorCode));
        return File(result.Data, "application/pdf", $"memo-{id}.pdf");
    }

    [HttpGet("{id}/generate-docx")]
    [Permission(MenuPermissionConstant.MemoGeneratePdf)]
    public async Task<IActionResult> GenerateDocxAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await service.GenerateDocxAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(SharedKernel.Operation.ErrorApiResponse.WrapError(result.Error, result.ErrorCode));
        return File(result.Data, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"memo-{id}.docx");
    }

    [HttpPost("export")]
    [Permission(MenuPermissionConstant.MemoExport)]
    public async Task<IActionResult> ExportAsync([FromBody] MemoListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var result = await service.ExportToExcelAsync(requestModel ?? new MemoListRequestModel(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(SharedKernel.Operation.ErrorApiResponse.WrapError(result.Error, result.ErrorCode));
        return File(result.Data, "text/csv", "memos.csv");
    }
}
