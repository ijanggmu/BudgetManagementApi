using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Fodo;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Fodo;

public class AdminFodoController(IFodoService fodoService) : BaseAdminApiController
{
    /// <summary>
    /// Get all fodos (Field Officer/Door Office Marketing) - for tenant admin: their tenant's fodos, for superadmin: all fodos or filtered by tenantId
    /// </summary>
    /// <param name="tenantId">Optional tenant ID filter (SuperAdmin only)</param>
    /// <returns>List of fodos</returns>
    [HttpPost]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? tenantId = null)
        => HandleResult(await fodoService.GetFodosForAdminAsync(tenantId));

    /// <summary>
    /// Get fodo by ID
    /// </summary>
    /// <param name="id">Fodo ID</param>
    /// <returns>Fodo details</returns>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutivesView)]
    public async Task<IActionResult> GetAsync(string id)
        => HandleResult(await fodoService.GetFodoByIdAsync(id));

    /// <summary>
    /// Create a new fodo (Field Officer/Door Office Marketing)
    /// </summary>
    /// <param name="dto">Fodo creation data</param>
    /// <returns>Created fodo details</returns>
    [HttpPost("create")]
    [Permission(MenuPermissionConstant.MarketingExecutivesCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFodoDto dto)
        => HandleResult(await fodoService.CreateAsync(dto));

    /// <summary>
    /// Update fodo
    /// </summary>
    /// <param name="id">Fodo ID</param>
    /// <param name="dto">Fodo update data</param>
    /// <returns>Updated fodo details</returns>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutivesUpdate)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateFodoDto dto)
        => HandleResult(await fodoService.UpdateAsync(id, dto));

    /// <summary>
    /// Delete fodo (soft delete)
    /// </summary>
    /// <param name="id">Fodo ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutivesDelete)]
    public async Task<IActionResult> DeleteAsync(string id)
        => HandleResult(await fodoService.DeleteAsync(id));
}
