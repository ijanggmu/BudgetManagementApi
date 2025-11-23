using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Quotation;

public class QuotationController(IQuotationService quotes) : BaseFoDoApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuotationDto dto)
    {
        return HandleResult(await quotes.CreateAsync(dto));
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel? requestModel = null)
    {
        return HandleResult(await quotes.ListAsync(requestModel));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await quotes.GetByIdAsync(id));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateQuotationDto dto)
    {
        return HandleResult(await quotes.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return HandleResult(await quotes.DeleteAsync(id));
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(string id)
    {
        return HandleResult(await quotes.GeneratePdfAsync(id));
    }
}


