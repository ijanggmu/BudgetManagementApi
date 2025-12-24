using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Branch;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Branch;

public class AdminBranchController(IBranchService branchService) : BaseAdminApiController
{
    /// <summary>
    /// Get all branches
    /// </summary>
    /// <param name="tenantId">Optional tenant ID filter (SuperAdmin only)</param>
    /// <returns>List of branches</returns>
    [HttpGet]
    [Permission(MenuPermissionConstant.BranchView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? tenantId = null, CancellationToken cancellationToken = default)
        => HandleResult(await branchService.GetAllAsync(tenantId, cancellationToken));

    /// <summary>
    /// Get branch by ID
    /// </summary>
    /// <param name="id">Branch ID</param>
    /// <returns>Branch details</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.BranchView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await branchService.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Create a new branch
    /// </summary>
    /// <param name="dto">Branch creation data</param>
    /// <returns>Created branch details</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.BranchCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBranchDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await branchService.CreateAsync(dto, cancellationToken));

    /// <summary>
    /// Update branch
    /// </summary>
    /// <param name="id">Branch ID</param>
    /// <param name="dto">Branch update data</param>
    /// <returns>Updated branch details</returns>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.BranchUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateBranchDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await branchService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>
    /// Delete branch (soft delete)
    /// </summary>
    /// <param name="id">Branch ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.BranchDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await branchService.DeleteAsync(id, cancellationToken));
}

