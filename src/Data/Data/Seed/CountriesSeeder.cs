using System.Text.Json;
using Data.Context;
using Data.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Models.Common.Country;
using SharedKernel.Constant.Country;

namespace Data.Seed;
public static class CountriesSeeder
{
    public async static Task SeedData(ApplicationDataContext dbContext)
    {
        List<CountrySeedModel> countryList = JsonSerializer.Deserialize<List<CountrySeedModel>>(CountriesJsonList.GetCountriesJsonList())!;

        countryList = countryList.OrderBy(x => x.name).ToList();
        CountrySeedModel nepal = countryList.FirstOrDefault(x => x.code == CountriesISO2.Nepal);
        countryList.Remove(nepal);

        List<CountrySeedModel> result = new() { nepal };
        result.AddRange(countryList);

        var countries = result.Select(x => new Country()
        {
            CountryName = x.name,
            CountryDialingCode = x.dial_code,
            CountryISO2 = x.code,
            CountryISO3 = CountriesISO3.GetActiveCountries().Where(c => c.Key == x.name).Select(x => x.Value).FirstOrDefault(),
            IsActive = CountriesISO2.GetActiveCountries().Any(c => c == x.code),
            CountryFlagIcon = $"https://flagcdn.com/72x54/{x.code.ToLower()}.png",
        }).ToList();

        if (!await dbContext.Countries.AnyAsync())
        {
            await dbContext.Countries.AddRangeAsync(countries);
            await dbContext.SaveChangesAsync();
        }
    }
    
}
