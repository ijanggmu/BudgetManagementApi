using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.TenantDomain;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Notification;

[Route("api/v1/notifications")]
public class NotificationController(INotificationUserService notificationService, IUserProfileService userProfileService) : BaseCommonApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.NotificationsView)]
    public async Task<IActionResult> GetMyNotificationsAsync([FromQuery] CommonPaginationRequestModel requestModel)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        return HandleResult(await notificationService.GetMyNotificationsAsync(requestModel, userId));
    }

    [HttpPatch("{id}/read")]
    [Permission(MenuPermissionConstant.NotificationsView)]
    public async Task<IActionResult> MarkAsReadAsync(string id)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        return HandleResult(await notificationService.MarkAsReadAsync(id, userId));
    }
}

