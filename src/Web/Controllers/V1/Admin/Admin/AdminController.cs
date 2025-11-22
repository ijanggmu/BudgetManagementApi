using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.Admin.Admin;


public class AdminController(IAdminService adminService) : BaseAdminApiController
{
    /// <summary>
    /// Get all admins (for tenant admin: their tenant's admins, for superadmin: all admins or filtered by tenantId)
    /// </summary>
    /// <param name="tenantId">Optional tenant ID filter (SuperAdmin only)</param>
    /// <returns>List of admins</returns>
    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] string? tenantId = null)
        => HandleResult(await adminService.GetAdminsForAdminAsync(tenantId));

    /// <summary>
    /// Get admin by ID
    /// </summary>
    /// <param name="id">Admin ID</param>
    /// <returns>Admin details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
        => HandleResult(await adminService.GetAdminByIdAsync(id));

    /// <summary>
    /// Create a new admin user
    /// </summary>
    /// <param name="dto">Admin creation data</param>
    /// <returns>Created admin details</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAdminDto dto)
        => HandleResult(await adminService.CreateAsync(dto));

    /// <summary>
    /// Update admin user
    /// </summary>
    /// <param name="id">Admin ID</param>
    /// <param name="dto">Admin update data</param>
    /// <returns>Updated admin details</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateAdminDto dto)
        => HandleResult(await adminService.UpdateAsync(id, dto));

    /// <summary>
    /// Delete admin user (soft delete)
    /// </summary>
    /// <param name="id">Admin ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
        => HandleResult(await adminService.DeleteAsync(id));
}

