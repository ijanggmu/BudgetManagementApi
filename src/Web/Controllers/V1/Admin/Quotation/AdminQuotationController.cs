using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace BeemaEdgeApi.Controllers.V1.Admin.Quotation;

public class AdminQuotationController(IQuotationService quotationService) : BaseAdminApiController
{
    /// <summary>
    /// Get all quotations for Tenant Admin (filtered by tenant) or SuperAdmin (all tenants)
    /// </summary>
    /// <param name="requestModel">Pagination and Sieve filter parameters</param>
    /// <param name="status">Optional: Filter by quotation status (Draft, Submitted, Approved, Declined, Accepted)</param>
    /// <param name="from">Optional: Filter quotations created from this date</param>
    /// <param name="to">Optional: Filter quotations created until this date</param>
    /// <returns>Paginated list of quotations</returns>
    [HttpGet]
    public async Task<IActionResult> GetQuotationsAsync(
        [FromQuery] CommonPaginationRequestModel requestModel,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return HandleResult(await quotationService.GetQuotationsForAdminAsync(requestModel, status, from, to));
    }

    /// <summary>
    /// Get quotation details by ID for Admin or SuperAdmin
    /// </summary>
    /// <param name="id">Quotation ID</param>
    /// <returns>Quotation details with items</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuotationDetailsAsync(string id)
    {
        return HandleResult(await quotationService.GetQuotationDetailsForAdminAsync(id));
    }

    /// <summary>
    /// Get quotations by tenantId (SuperAdmin only)
    /// </summary>
    /// <param name="tenantId">Tenant ID to filter quotations</param>
    /// <param name="requestModel">Pagination and Sieve filter parameters</param>
    /// <param name="status">Optional: Filter by quotation status (Draft, Submitted, Approved, Declined, Accepted)</param>
    /// <param name="from">Optional: Filter quotations created from this date</param>
    /// <param name="to">Optional: Filter quotations created until this date</param>
    /// <returns>Paginated list of quotations for the specified tenant</returns>
    [HttpGet("tenant/{tenantId}")]
    public async Task<IActionResult> GetQuotationsByTenantIdAsync(
        string tenantId,
        [FromQuery] CommonPaginationRequestModel requestModel,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return HandleResult(await quotationService.GetQuotationsByTenantIdAsync(tenantId, requestModel, status, from, to));
    }

    /// <summary>
    /// Export quotations to Excel
    /// </summary>
    /// <param name="status">Optional: Filter by quotation status</param>
    /// <param name="from">Optional: Filter quotations created from this date</param>
    /// <param name="to">Optional: Filter quotations created until this date</param>
    /// <param name="tenantId">Optional: Tenant ID filter (SuperAdmin only)</param>
    /// <returns>Excel file</returns>
    [HttpGet("export")]
    public async Task<IActionResult> ExportToExcelAsync(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? tenantId = null)
    {
        var result = await quotationService.ExportToExcelAsync(status, from, to, tenantId);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Quotations_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

