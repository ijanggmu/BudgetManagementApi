using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.ApprovalConfig;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.ApprovalConfig;

[AdminOrSuperAdmin]
public class ApprovalConfigController(IApprovalConfigService service) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.ApprovalConfigView)]
    public async Task<IActionResult> ListAsync([FromBody] ApprovalConfigListRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(requestModel ?? new ApprovalConfigListRequestModel(), cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.ApprovalConfigView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpGet("department/{departmentId}")]
    [Permission(MenuPermissionConstant.ApprovalConfigView)]
    public async Task<IActionResult> GetByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByDepartmentIdAsync(departmentId, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.ApprovalConfigCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateApprovalConfigDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.ApprovalConfigUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateApprovalConfigDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.ApprovalConfigDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));

    [HttpPost("import")]
    [Permission(MenuPermissionConstant.ApprovalConfigCreate)]
    public async Task<IActionResult> ImportAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a CSV file.");
        if (!string.Equals(Path.GetExtension(file.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only CSV files are supported.");
        await using var stream = file.OpenReadStream();
        return HandleResult(await service.ImportFromCsvAsync(stream, cancellationToken));
    }
}
