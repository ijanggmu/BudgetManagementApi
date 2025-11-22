using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

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
    [HttpGet]
    public async Task<IActionResult> GetLeadsAsync(
        [FromQuery] CommonPaginationRequestModel requestModel,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return HandleResult(await leadService.GetLeadsForAdminAsync(requestModel, status, from, to));
    }

    /// <summary>
    /// Get lead details by ID for Admin or SuperAdmin
    /// </summary>
    /// <param name="id">Lead ID</param>
    /// <returns>Lead details with prospect and contact information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeadDetailsAsync(string id)
    {
        return HandleResult(await leadService.GetLeadDetailsForAdminAsync(id));
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
    public async Task<IActionResult> GetLeadsByTenantIdAsync(
        string tenantId,
        [FromQuery] CommonPaginationRequestModel requestModel,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return HandleResult(await leadService.GetLeadsByTenantIdAsync(tenantId, requestModel, status, from, to));
    }
}

