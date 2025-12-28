using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Quotation;

[AdminOrSuperAdmin] // All endpoints require Admin or SuperAdmin role
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
    [HttpPost]
    [Permission(MenuPermissionConstant.AdminQuotationsView)]
    public async Task<IActionResult> GetQuotationsAsync(
        [FromBody] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotationService.GetQuotationsForAdminAsync(requestModel, cancellationToken));
    }

    /// <summary>
    /// Get quotation details by ID for Admin or SuperAdmin
    /// </summary>
    /// <param name="id">Quotation ID</param>
    /// <returns>Quotation details with items</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.AdminQuotationsView)]
    public async Task<IActionResult> GetQuotationDetailsAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotationService.GetQuotationDetailsForAdminAsync(id, cancellationToken));
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
    [SuperAdminOnly] // Only SuperAdmin can filter by tenantId
    [Permission(MenuPermissionConstant.AdminQuotationsView)]
    public async Task<IActionResult> GetQuotationsByTenantIdAsync(
        string tenantId,
        [FromQuery] CommonPaginationRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await quotationService.GetQuotationsByTenantIdAsync(tenantId, requestModel, cancellationToken));
    }

    /// <summary>
    /// Export quotations to Excel
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file</returns>
    [HttpPost("export")]
    [Permission(MenuPermissionConstant.AdminQuotationsExport)]
    public async Task<IActionResult> ExportToExcelAsync(
        [FromBody] CommonPaginationRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        var result = await quotationService.ExportToExcelAsync(requestModel, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Quotations_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

