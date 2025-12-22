using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForQuotationItem : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<QuotationItem>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<QuotationItem>(p => p.QuotationId)
           .CanFilter()
           .CanSort();

        mapper.Property<QuotationItem>(p => p.CoverageId)
          .CanFilter()
          .CanSort();

        mapper.Property<QuotationItem>(p => p.SumInsured)
          .CanFilter()
          .CanSort();

        mapper.Property<QuotationItem>(p => p.Premium)
          .CanFilter()
          .CanSort();

        mapper.Property<QuotationItem>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<QuotationItem>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<QuotationItem>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<QuotationItem>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<QuotationItem>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

