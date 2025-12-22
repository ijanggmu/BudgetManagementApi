using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForDocumentTemplate : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<DocumentTemplate>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<DocumentTemplate>(p => p.Name)
           .CanFilter()
           .CanSort();

        mapper.Property<DocumentTemplate>(p => p.Kind)
          .CanFilter()
          .CanSort();

        mapper.Property<DocumentTemplate>(p => p.ContentUri)
          .CanFilter()
          .CanSort();

        mapper.Property<DocumentTemplate>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<DocumentTemplate>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<DocumentTemplate>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<DocumentTemplate>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<DocumentTemplate>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

