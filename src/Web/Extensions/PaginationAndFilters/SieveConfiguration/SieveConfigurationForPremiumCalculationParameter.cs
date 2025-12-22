using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForPremiumCalculationParameter : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<PremiumCalculationParameter>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.ConfigurationId)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.ParameterKey)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.ParameterName)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.DataType)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.Value)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.DefaultValue)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.MinValue)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.MaxValue)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.IsRequired)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.DisplayOrder)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.Category)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<PremiumCalculationParameter>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

