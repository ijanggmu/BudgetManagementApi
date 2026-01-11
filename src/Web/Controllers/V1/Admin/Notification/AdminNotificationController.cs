using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.AdminPortalApi.Notification;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.Notification;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Notification;

[Route("api/v1/admin/notifications")]
public class AdminNotificationController(INotificationService notificationService) : BaseAdminApiController
{
    /// <summary>
    /// Send notification to all FoDo/Marketing Executives in the tenant
    /// </summary>
    [HttpPost("send")]
    [Permission(MenuPermissionConstant.NotificationsView)]
    public async Task<IActionResult> SendNotificationAsync(
        [FromBody] SendNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await notificationService.SendNotificationAsync(dto, cancellationToken));
    }

    /// <summary>
    /// Get current user's notifications with pagination
    /// </summary>
    [HttpPost("list")]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> GetUserNotificationsAsync(
        [FromBody] CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await notificationService.GetUserNotificationsAsync(requestModel, cancellationToken));
    }

    /// <summary>
    /// Get unread notification count for current user
    /// </summary>
    [HttpGet("unread-count")]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        return HandleResult(await notificationService.GetUnreadCountAsync(cancellationToken));
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPatch("{id}/read")]
    [Permission(MenuPermissionConstant.DashboardView)]
    public async Task<IActionResult> MarkAsReadAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await notificationService.MarkAsReadAsync(id, cancellationToken));
    }
}

