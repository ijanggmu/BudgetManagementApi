using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForNotificationHistory : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<NotificationHistory>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<NotificationHistory>(p => p.UserId)
           .CanFilter()
           .CanSort();

        mapper.Property<NotificationHistory>(p => p.Channel)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.Recipient)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.Subject)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.Body)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.SentAt)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.IsSuccess)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.ErrorMessage)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<NotificationHistory>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<NotificationHistory>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<NotificationHistory>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<NotificationHistory>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

