using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;
using Business.Common.TenantDomain;
using Models.Common;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.Common.Quotation;

[Route("api/v1/quotations")]
public class QuotationController(IQuotationService quotes) : BaseCommonApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuotationDto dto)
    {
        return HandleResult(await quotes.CreateAsync(dto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await quotes.GetByIdAsync(id));
    }


    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(string id)
    {
        return HandleResult(await quotes.GeneratePdfAsync(id));
    }
}


