using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForMerchant : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Fodo>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Vendor>(p => p.VendorName)
           .CanFilter()
           .CanSort();

        mapper.Property<Vendor>(p => p.Email)
          .CanFilter()
          .CanSort();

        mapper.Property<Vendor>(p => p.PhoneNumber)
          .CanFilter()
          .CanSort();
    }
}
