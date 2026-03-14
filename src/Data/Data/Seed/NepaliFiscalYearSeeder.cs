using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Data.Seed;

/// <summary>
/// Seeds Nepali fiscal years (23/24, 24/25, 25/26, etc.) for all tenants.
/// </summary>
public static class NepaliFiscalYearSeeder
{
    /// <summary>
    /// Fiscal years to seed: code, start year, end year. Add more as needed.
    /// </summary>
    private static readonly (string Code, int StartYear, int EndYear)[] DefaultFiscalYears =
    {
        ("23/24", 2023, 2024),
        ("24/25", 2024, 2025),
        ("25/26", 2025, 2026),
        ("26/27", 2026, 2027),
        ("27/28", 2027, 2028),
        ("28/29", 2028, 2029),
        ("29/30", 2029, 2030),
        ("30/31", 2030, 2031),
    };

    public static async Task SeedAsync(ApplicationDataContext context)
    {
        var tenants = await context.Tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new { t.Id })
            .ToListAsync();

        foreach (var t in tenants)
        {
            foreach (var (code, startYear, endYear) in DefaultFiscalYears)
            {
                var exists = await context.NepaliFiscalYears
                    .IgnoreQueryFilters()
                    .AnyAsync(f => f.TenantId == t.Id && f.Code == code);
                if (exists)
                    continue;

                await context.NepaliFiscalYears.AddAsync(new NepaliFiscalYear
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = t.Id,
                    Code = code,
                    StartYear = startYear,
                    EndYear = endYear,
                    Description = $"Nepali fiscal year {code}",
                    CreatedOn = DateTime.UtcNow,
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
