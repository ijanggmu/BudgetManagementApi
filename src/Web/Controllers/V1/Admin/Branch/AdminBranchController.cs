using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Branch;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Branch;

[AdminOrSuperAdmin] // All endpoints require Admin or SuperAdmin role
public class AdminBranchController(IBranchService branchService) : BaseAdminApiController
{
    /// <summary>
    /// Get all branches
    /// </summary>
    /// <param name="requestModel">Pagination and Sieve filter parameters</param>
    /// <returns>Paginated list of branches</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.BranchView)]
    public async Task<IActionResult> ListAsync([FromBody] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await branchService.GetAllAsync(requestModel, cancellationToken));

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
    [HttpPost("create")]
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

    /// <summary>
    /// Import branches from Excel file
    /// </summary>
    /// <param name="file">Excel file (.xlsx) containing branches</param>
    /// <returns>Import result with success/failure counts and errors</returns>
    [HttpPost("import")]
    [RequestSizeLimit(10 * 1024 * 1024)] // Limit to 10MB max
    [Permission(MenuPermissionConstant.BranchCreate)]
    public async Task<IActionResult> ImportAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx files are supported.");

        await using var stream = file.OpenReadStream();
        return HandleResult(await branchService.ImportFromExcelAsync(stream, cancellationToken));
    }
}

