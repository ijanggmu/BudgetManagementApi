using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForRenewalReminder : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<RenewalReminder>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<RenewalReminder>(p => p.PolicyId)
           .CanFilter()
           .CanSort();

        mapper.Property<RenewalReminder>(p => p.UserId)
          .CanFilter()
          .CanSort();

        mapper.Property<RenewalReminder>(p => p.DueDate)
          .CanFilter()
          .CanSort();

        mapper.Property<RenewalReminder>(p => p.ReminderSentAt)
          .CanFilter()
          .CanSort();

        mapper.Property<RenewalReminder>(p => p.Channel)
          .CanFilter()
          .CanSort();

        mapper.Property<RenewalReminder>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<RenewalReminder>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<RenewalReminder>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<RenewalReminder>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<RenewalReminder>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

