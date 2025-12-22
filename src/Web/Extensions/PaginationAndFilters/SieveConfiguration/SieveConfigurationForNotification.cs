using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForNotification : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Notification>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Notification>(p => p.UserId)
           .CanFilter()
           .CanSort();

        mapper.Property<Notification>(p => p.Title)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.Body)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.Channel)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.Payload)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.SentAt)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.ReadAt)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Notification>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Notification>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Notification>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Notification>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

