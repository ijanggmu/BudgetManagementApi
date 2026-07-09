using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Memo;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.MemoTemplate;

[BmsPortalUser]
public class MemoTemplateController(IMemoTemplateService service) : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.MemoView)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken = default)
        => HandleResult(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.MemoView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost("create")]
    [Permission(MenuPermissionConstant.MemoCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMemoTemplateDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.CreateAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.MemoUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateMemoTemplateDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await service.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.MemoDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await service.DeleteAsync(id, cancellationToken));
}
