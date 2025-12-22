using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForLead : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Lead>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Lead>(p => p.ProspectId)
           .CanFilter()
           .CanSort();

        mapper.Property<Lead>(p => p.Status)
          .CanFilter()
          .CanSort();

        mapper.Property<Lead>(p => p.Source)
          .CanFilter()
          .CanSort();

        mapper.Property<Lead>(p => p.OwnerUserId)
          .CanFilter()
          .CanSort();

        mapper.Property<Lead>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Lead>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Lead>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Lead>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Lead>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

