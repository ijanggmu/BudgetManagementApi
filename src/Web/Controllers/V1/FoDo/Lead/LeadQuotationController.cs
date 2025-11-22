using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Lead;

[Route("api/v1/leads")]
public class LeadQuotationController : BaseFoDoApiController
{
    private readonly IQuotationService _quotationService;

    public LeadQuotationController(IQuotationService quotationService)
    {
        _quotationService = quotationService;
    }
    [HttpGet("{leadId}/quotations")]
    public async Task<IActionResult> GetQuotationsByLeadIdAsync(string leadId)
    {
        return HandleResult(await _quotationService.GetByLeadIdAsync(leadId));
    }
}

