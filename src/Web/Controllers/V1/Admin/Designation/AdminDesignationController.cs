using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Designation;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Designation;

public class AdminDesignationController(IDesignationService designationService) : BaseAdminApiController
{
    /// <summary>
    /// Get all designations
    /// </summary>
    /// <param name="tenantId">Optional tenant ID filter (SuperAdmin only)</param>
    /// <returns>List of designations</returns>
    [HttpGet]
    [Permission(MenuPermissionConstant.DesignationView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? tenantId = null, CancellationToken cancellationToken = default)
        => HandleResult(await designationService.GetAllAsync(tenantId, cancellationToken));

    /// <summary>
    /// Get designation by ID
    /// </summary>
    /// <param name="id">Designation ID</param>
    /// <returns>Designation details</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.DesignationView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await designationService.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Create a new designation
    /// </summary>
    /// <param name="dto">Designation creation data</param>
    /// <returns>Created designation details</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.DesignationCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDesignationDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await designationService.CreateAsync(dto, cancellationToken));

    /// <summary>
    /// Update designation
    /// </summary>
    /// <param name="id">Designation ID</param>
    /// <param name="dto">Designation update data</param>
    /// <returns>Updated designation details</returns>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.DesignationUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateDesignationDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await designationService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>
    /// Delete designation (soft delete)
    /// </summary>
    /// <param name="id">Designation ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.DesignationDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await designationService.DeleteAsync(id, cancellationToken));
}

