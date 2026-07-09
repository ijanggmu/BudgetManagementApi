using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.BudgetMemoAudit;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.BudgetMemoAudit;

[BmsPortalUser]
[Route("api/v{version:apiVersion}/BudgetMemoAudit")]
[ApiVersion("1.0")]
public class BudgetMemoAuditController(IBudgetMemoAuditService service) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.BudgetView)]
    public async Task<IActionResult> ListAsync([FromBody] BudgetMemoAuditListRequestModel request, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetListAsync(request ?? new BudgetMemoAuditListRequestModel(), cancellationToken));
}
