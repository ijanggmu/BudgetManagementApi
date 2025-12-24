using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.MarketingExecutive.Lead;

public class LeadQuotationController : BaseMarketingExecutiveApiController
{
    private readonly IQuotationService _quotationService;

    public LeadQuotationController(IQuotationService quotationService)
    {
        _quotationService = quotationService;
    }
    [HttpGet("{leadId}/quotations")]
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsView)]
    public async Task<IActionResult> GetQuotationsByLeadIdAsync(string leadId, CancellationToken cancellationToken = default)
    {
        return HandleResult(await _quotationService.GetByLeadIdAsync(leadId, cancellationToken));
    }
}

