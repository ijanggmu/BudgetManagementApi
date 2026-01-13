using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Quotation;

public class QuotationController(IQuotationService quotes) : BaseMarketingExecutiveApiController
{
    [HttpPost("create")]
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuotationDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.CreateAsync(dto, cancellationToken));
    }

    [HttpPost]
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsView)]
    public async Task<IActionResult> ListAsync([FromBody] CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.ListAsync(requestModel, cancellationToken));
    }

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.GetByIdAsync(id, cancellationToken));
    }

    [HttpPatch("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateQuotationDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotes.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsDelete)]
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
    [Permission(MenuPermissionConstant.MarketingExecutiveQuotationsView)]
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


