using Data.Entities.AdminEntity;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForAdmin : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Admin>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Admin>(p => p.FullName)
           .CanFilter()
           .CanSort();

        mapper.Property<Admin>(p => p.User.Email)
           .CanFilter()
           .CanSort();

        mapper.Property<Admin>(p => p.User.UserName)
           .CanFilter()
           .CanSort();

        mapper.Property<Admin>(p => p.User.PhoneNumber)
           .CanFilter()
           .CanSort();
    }
}
