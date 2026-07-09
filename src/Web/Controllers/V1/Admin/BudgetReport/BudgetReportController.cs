using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.BudgetReport;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.BudgetReport;

[BmsPortalUser]
public class BudgetReportController(IBudgetReportService service) : BaseAdminApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.BudgetReportView)]
    public async Task<IActionResult> GetReportAsync([FromBody] BudgetReportRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await service.GetReportAsync(requestModel ?? new BudgetReportRequestModel(), cancellationToken));
}
