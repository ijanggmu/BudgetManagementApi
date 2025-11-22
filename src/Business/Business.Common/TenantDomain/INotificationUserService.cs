using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Tenant;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface INotificationUserService
{
    Task<Result<List<Notification>>> GetMyNotificationsAsync(CommonPaginationRequestModel requestModel, string userId);
    Task<Result<bool>> MarkAsReadAsync(string id, string userId);
}

