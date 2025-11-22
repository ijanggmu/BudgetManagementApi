using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class NotificationUserService : INotificationUserService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;

    public NotificationUserService(ApplicationDataContext db, ISieveExtension sieveExtension)
    {
        _db = db;
        _sieveExtension = sieveExtension;
    }

    public async Task<Result<List<Notification>>> GetMyNotificationsAsync(CommonPaginationRequestModel requestModel, string userId)
    {
        var query = _db.Set<Notification>()
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt);

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
        var notifications = await result.ToListAsync();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<Notification>>.Success(notifications, pagination);
    }

    public async Task<Result<bool>> MarkAsReadAsync(string id, string userId)
    {
        var notification = await _db.Set<Notification>()
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null)
            return Result<bool>.Failed("Notification not found.");

        if (!notification.ReadAt.HasValue)
        {
            notification.ReadAt = System.DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return Result<bool>.Success(true);
    }
}

