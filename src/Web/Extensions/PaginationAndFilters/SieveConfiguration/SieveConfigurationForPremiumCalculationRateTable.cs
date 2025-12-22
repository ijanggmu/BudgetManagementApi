using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPremiumCalculationRateTable : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<PremiumCalculationRateTable>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.ConfigurationId)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.TableName)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.LookupKey)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationRateTable>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

