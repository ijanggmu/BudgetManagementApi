using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
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
}
