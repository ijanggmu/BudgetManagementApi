using System;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Fodo;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.MarketingExecutive;

public class AdminMarketingExecutiveController(IFodoService fodoService) : BaseAdminApiController
{
    /// <summary>
    /// Get all marketing executives (for tenant admin: their tenant's executives, for superadmin: all executives or filtered by tenantId)
    /// </summary>
    /// <param name="tenantId">Optional tenant ID filter (SuperAdmin only)</param>
    /// <returns>List of marketing executives</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> ListAsync([FromBody] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.GetFodosForAdminAsync(requestModel, cancellationToken));

    /// <summary>
    /// Get marketing executive by ID
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <returns>Marketing Executive details</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.GetFodoByIdAsync(id, cancellationToken));

    /// <summary>
    /// Create a new marketing executive
    /// </summary>
    /// <param name="dto">Marketing Executive creation data</param>
    /// <returns>Created marketing executive details</returns>
    [HttpPost("create")]
    [Permission(MenuPermissionConstant.MarketingExecutivesCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFodoDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.CreateAsync(dto, cancellationToken));

    /// <summary>
    /// Update marketing executive
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <param name="dto">Marketing Executive update data</param>
    /// <returns>Updated marketing executive details</returns>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutivesUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateFodoDto dto, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.UpdateAsync(id, dto, cancellationToken));

    /// <summary>
    /// Delete marketing executive (soft delete)
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutivesDelete)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.DeleteAsync(id, cancellationToken));

    /// <summary>
    /// Change password for marketing executive
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <param name="request">Password change request</param>
    /// <returns>Success message</returns>
    [HttpPut("{id}/change-password")]
    [Permission(MenuPermissionConstant.MarketingExecutivesUpdate)]
    public async Task<IActionResult> ChangePasswordAsync(string id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.ChangePasswordAsync(id, request.NewPassword, cancellationToken));

    /// <summary>
    /// Enable or disable marketing executive user
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <param name="request">Status change request</param>
    /// <returns>Success message</returns>
    [HttpPut("{id}/toggle-status")]
    [Permission(MenuPermissionConstant.MarketingExecutivesUpdate)]
    public async Task<IActionResult> ToggleStatusAsync(string id, [FromBody] ToggleStatusRequest request, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.ToggleUserStatusAsync(id, request.IsDisabled, cancellationToken));

    /// <summary>
    /// Get access logs for marketing executive
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <param name="requestModel">Pagination request</param>
    /// <returns>List of access logs</returns>
    [HttpGet("{id}/access-logs")]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> GetAccessLogsAsync(string id, [FromQuery] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.GetAccessLogsAsync(id, requestModel, cancellationToken));

    /// <summary>
    /// Get leads for marketing executive
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <param name="requestModel">Pagination request</param>
    /// <returns>List of leads</returns>
    [HttpGet("{id}/leads")]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> GetLeadsAsync(string id, [FromQuery] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.GetLeadsAsync(id, requestModel, cancellationToken));

    /// <summary>
    /// Get quotations for marketing executive
    /// </summary>
    /// <param name="id">Marketing Executive ID</param>
    /// <param name="requestModel">Pagination request</param>
    /// <returns>List of quotations</returns>
    [HttpGet("{id}/quotations")]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> GetQuotationsAsync(string id, [FromQuery] CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
        => HandleResult(await fodoService.GetQuotationsAsync(id, requestModel, cancellationToken));

    /// <summary>
    /// Import marketing executives from Excel file
    /// </summary>
    /// <param name="file">Excel file (.xlsx) containing marketing executives</param>
    /// <returns>Import result with success/failure counts and errors</returns>
    [HttpPost("import")]
    [RequestSizeLimit(10 * 1024 * 1024)] // Limit to 10MB max
    [Permission(MenuPermissionConstant.MarketingExecutivesCreate)]
    public async Task<IActionResult> ImportAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx files are supported.");

        await using var stream = file.OpenReadStream();
        return HandleResult(await fodoService.ImportFromExcelAsync(stream, cancellationToken));
    }
}

public class ChangePasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

public class ToggleStatusRequest
{
    public bool IsDisabled { get; set; }
}

