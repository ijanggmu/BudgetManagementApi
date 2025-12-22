using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Quotation;

public class QuotationController(IQuotationService quotes) : BaseMarketingExecutiveApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuotationDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.CreateAsync(dto, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.ListAsync(requestModel, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.GetByIdAsync(id, cancellationToken));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateQuotationDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.DeleteAsync(id, cancellationToken));
    }

    /// <summary>
    /// Generate and download quotation PDF
    /// </summary>
    /// <param name="id">Quotation ID</param>
    /// <returns>PDF file</returns>
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await quotes.GeneratePdfAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var quotation = await quotes.GetByIdAsync(id, cancellationToken);
        var quotationNumber = quotation.IsSuccess && quotation.Data != null 
            ? quotation.Data.Number 
            : id;

        var fileName = $"Quotation_{quotationNumber}_{DateTime.UtcNow:yyyyMMdd}.pdf";
        return File(result.Data, "application/pdf", fileName);
    }
}


