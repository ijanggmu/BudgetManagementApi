using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Lead;

public class AdminLeadController(ILeadService leadService) : BaseAdminApiController
{
    /// <summary>
    /// Get all leads for Tenant Admin (filtered by tenant) or SuperAdmin (all tenants)
    /// </summary>
    /// <param name="requestModel">Pagination and Sieve filter parameters</param>
    /// <param name="status">Optional: Filter by lead status (New, Qualified, Contacted, Quoted, Won, Lost)</param>
    /// <param name="from">Optional: Filter leads created from this date</param>
    /// <param name="to">Optional: Filter leads created until this date</param>
    /// <returns>Paginated list of leads</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.AdminLeadsView)]
    public async Task<IActionResult> GetLeadsAsync(
        [FromQuery] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leadService.GetLeadsForAdminAsync(requestModel, cancellationToken));
    }

    /// <summary>
    /// Get lead details by ID for Admin or SuperAdmin
    /// </summary>
    /// <param name="id">Lead ID</param>
    /// <returns>Lead details with prospect and contact information</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.AdminLeadsView)]
    public async Task<IActionResult> GetLeadDetailsAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leadService.GetLeadDetailsForAdminAsync(id, cancellationToken));
    }

    /// <summary>
    /// Get leads by tenantId (SuperAdmin only)
    /// </summary>
    /// <param name="tenantId">Tenant ID to filter leads</param>
    /// <param name="requestModel">Pagination and Sieve filter parameters</param>
    /// <param name="status">Optional: Filter by lead status (New, Qualified, Contacted, Quoted, Won, Lost)</param>
    /// <param name="from">Optional: Filter leads created from this date</param>
    /// <param name="to">Optional: Filter leads created until this date</param>
    /// <returns>Paginated list of leads for the specified tenant</returns>
    [HttpGet("tenant/{tenantId}")]
    [Permission(MenuPermissionConstant.AdminLeadsView)]
    public async Task<IActionResult> GetLeadsByTenantIdAsync(
        string tenantId,
        [FromQuery] CommonPaginationRequestModel requestModel,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await leadService.GetLeadsByTenantIdAsync(tenantId, requestModel, cancellationToken));
    }

    /// <summary>
    /// Export leads to Excel
    /// </summary>
    /// <param name="requestModel">Pagination and filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file</returns>
    [HttpPost("export")]
    [Permission(MenuPermissionConstant.AdminLeadsExport)]
    public async Task<IActionResult> ExportToExcelAsync(
        [FromBody] CommonPaginationRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        var result = await leadService.ExportToExcelAsync(requestModel, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var fileName = $"Leads_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

