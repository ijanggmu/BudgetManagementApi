using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Data.Seed;

/// <summary>
/// Seeds fiscal years (23/24, 24/25, etc.) for all tenants. Dates stored in UTC.
/// </summary>
public static class NepaliFiscalYearSeeder
{
    /// <summary>
    /// Fiscal years to seed: code, start UTC, end UTC. Nepali FY typically starts mid-July.
    /// </summary>
    private static readonly (string Code, DateTime StartDateUtc, DateTime EndDateUtc)[] DefaultFiscalYears =
    {
        ("23/24", new DateTime(2023, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("24/25", new DateTime(2024, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("25/26", new DateTime(2025, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("26/27", new DateTime(2026, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2027, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("27/28", new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2028, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("28/29", new DateTime(2028, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2029, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("29/30", new DateTime(2029, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2030, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
        ("30/31", new DateTime(2030, 7, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2031, 7, 15, 23, 59, 59, DateTimeKind.Utc).AddMilliseconds(999)),
    };

    public static async Task SeedAsync(ApplicationDataContext context)
    {
        var tenants = await context.Tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new { t.Id })
            .ToListAsync();

        foreach (var t in tenants)
        {
            foreach (var (code, startDateUtc, endDateUtc) in DefaultFiscalYears)
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
                    StartDateUtc = startDateUtc,
                    EndDateUtc = endDateUtc,
                    Description = $"Fiscal year {code}",
                    CreatedOn = DateTime.UtcNow,
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
