using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Common.Country;
using Sieve.Services;

namespace Data.Entities.Common;
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasIndex(p => p.CountryName).IsUnique(true);
        builder.HasIndex(p => p.CountryISO2).IsUnique(true);
        builder.HasKey(p => p.Id);
    }
}

[EntityTypeConfiguration(typeof(CountryConfiguration))]
public class Country
{
    public int Id { get; set; }
    public string CountryName { get; set; }
    public string CountryISO2 { get; set; }
    public string CountryISO3 { get; set; }
    public string CountryDialingCode { get; set; }
    public string CountryFlagIcon { get; set; }
    public bool IsActive { get; set; }
}

public class SieveConfigurationForCountry : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {

        mapper.Property<Country>(p => p.Id)
          .CanFilter()
          .CanSort()
          .HasName(nameof(CountryResponseModel.CountryId));

        mapper.Property<Country>(p => p.CountryName)
          .CanFilter()
          .CanSort();

        mapper.Property<Country>(p => p.CountryDialingCode)
          .CanFilter()
          .CanSort();

        mapper.Property<Country>(p => p.CountryISO2)
          .CanFilter()
          .CanSort();

        mapper.Property<Country>(p => p.CountryISO3)
          .CanFilter()
          .CanSort();

        mapper.Property<Country>(p => p.CountryDialingCode)
          .CanFilter()
          .CanSort();

        mapper.Property<Country>(p => p.CountryFlagIcon)
          .CanFilter()
          .CanSort();

        mapper.Property<Country>(p => p.IsActive)
          .CanFilter()
          .CanSort();

    }
}
