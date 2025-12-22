using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForLeadActivity : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<LeadActivity>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<LeadActivity>(p => p.LeadId)
           .CanFilter()
           .CanSort();

        mapper.Property<LeadActivity>(p => p.Kind)
          .CanFilter()
          .CanSort();

        mapper.Property<LeadActivity>(p => p.Notes)
          .CanFilter()
          .CanSort();

        mapper.Property<LeadActivity>(p => p.When)
          .CanFilter()
          .CanSort();

        mapper.Property<LeadActivity>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<LeadActivity>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<LeadActivity>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<LeadActivity>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<LeadActivity>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

