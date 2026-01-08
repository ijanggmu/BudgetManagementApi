using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.AdminPortalApi.Gateway;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.Gateway;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Gateway;

[Route("api/v1/admin/gateway")]
public class AdminGatewayConfigurationController(IGatewayConfigurationService service) : BaseAdminApiController
{
    #region Email Gateway

    /// <summary>
    /// Get all email gateway configurations with pagination
    /// </summary>
    [HttpPost("email/list")]
    [Permission(MenuPermissionConstant.EmailGatewayView)]
    public async Task<IActionResult> GetAllEmailGatewaysAsync(
        [FromBody] CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetAllEmailGatewaysAsync(requestModel, cancellationToken));
    }

    /// <summary>
    /// Get email gateway configuration by ID
    /// </summary>
    [HttpGet("email/{id}")]
    [Permission(MenuPermissionConstant.EmailGatewayView)]
    public async Task<IActionResult> GetEmailGatewayByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetEmailGatewayByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Get active email gateway configuration
    /// </summary>
    [HttpGet("email/active")]
    [Permission(MenuPermissionConstant.EmailGatewayView)]
    public async Task<IActionResult> GetActiveEmailGatewayAsync(
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetActiveEmailGatewayAsync(cancellationToken));
    }

    /// <summary>
    /// Create new email gateway configuration
    /// </summary>
    [HttpPost("email")]
    [Permission(MenuPermissionConstant.EmailGatewayCreate)]
    public async Task<IActionResult> CreateEmailGatewayAsync(
        [FromBody] CreateEmailGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CreateEmailGatewayAsync(dto, cancellationToken));
    }

    /// <summary>
    /// Update email gateway configuration
    /// </summary>
    [HttpPatch("email/{id}")]
    [Permission(MenuPermissionConstant.EmailGatewayUpdate)]
    public async Task<IActionResult> UpdateEmailGatewayAsync(
        string id,
        [FromBody] UpdateEmailGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.UpdateEmailGatewayAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Delete email gateway configuration
    /// </summary>
    [HttpDelete("email/{id}")]
    [Permission(MenuPermissionConstant.EmailGatewayDelete)]
    public async Task<IActionResult> DeleteEmailGatewayAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.DeleteEmailGatewayAsync(id, cancellationToken));
    }

    #endregion

    #region SMS Gateway

    /// <summary>
    /// Get all SMS gateway configurations with pagination
    /// </summary>
    [HttpPost("sms/list")]
    [Permission(MenuPermissionConstant.SmsGatewayView)]
    public async Task<IActionResult> GetAllSmsGatewaysAsync(
        [FromBody] CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetAllSmsGatewaysAsync(requestModel, cancellationToken));
    }

    /// <summary>
    /// Get SMS gateway configuration by ID
    /// </summary>
    [HttpGet("sms/{id}")]
    [Permission(MenuPermissionConstant.SmsGatewayView)]
    public async Task<IActionResult> GetSmsGatewayByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetSmsGatewayByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Get active SMS gateway configuration
    /// </summary>
    [HttpGet("sms/active")]
    [Permission(MenuPermissionConstant.SmsGatewayView)]
    public async Task<IActionResult> GetActiveSmsGatewayAsync(
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.GetActiveSmsGatewayAsync(cancellationToken));
    }

    /// <summary>
    /// Create new SMS gateway configuration
    /// </summary>
    [HttpPost("sms")]
    [Permission(MenuPermissionConstant.SmsGatewayCreate)]
    public async Task<IActionResult> CreateSmsGatewayAsync(
        [FromBody] CreateSmsGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.CreateSmsGatewayAsync(dto, cancellationToken));
    }

    /// <summary>
    /// Update SMS gateway configuration
    /// </summary>
    [HttpPatch("sms/{id}")]
    [Permission(MenuPermissionConstant.SmsGatewayUpdate)]
    public async Task<IActionResult> UpdateSmsGatewayAsync(
        string id,
        [FromBody] UpdateSmsGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.UpdateSmsGatewayAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Delete SMS gateway configuration
    /// </summary>
    [HttpDelete("sms/{id}")]
    [Permission(MenuPermissionConstant.SmsGatewayDelete)]
    public async Task<IActionResult> DeleteSmsGatewayAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await service.DeleteSmsGatewayAsync(id, cancellationToken));
    }

    #endregion
}

