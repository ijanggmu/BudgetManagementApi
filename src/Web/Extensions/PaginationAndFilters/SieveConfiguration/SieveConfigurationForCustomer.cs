using Data.Entities.CustomerEntity;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForCustomer : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {

        mapper.Property<Customer>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Customer>(p => p.FullName)
          .CanFilter()
          .CanSort();

        mapper.Property<Customer>(p => p.Gender)
          .CanFilter()
          .CanSort();

        mapper.Property<Customer>(p => p.KycStatus)
         .CanFilter()
         .CanSort();

        mapper.Property<Customer>(p => p.KycRejectedReason)
         .CanFilter()
         .CanSort();

        mapper.Property<Customer>(p => p.Gender)
         .CanFilter()
         .CanSort();
    }
}
