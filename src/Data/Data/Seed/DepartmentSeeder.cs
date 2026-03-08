using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Data.Seed;

/// <summary>
/// Seeds default departments for the dummy tenant (and any tenant with no departments).
/// </summary>
public static class DepartmentSeeder
{
    private static readonly (string Name, string? Description)[] DefaultDepartments =
    {
        ("Finance", "Finance and accounting"),
        ("Operations", "Operations and delivery"),
        ("HR", "Human resources"),
        ("IT", "Information technology"),
    };

    public static async Task SeedAsync(ApplicationDataContext context)
    {
        var tenants = await context.Tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new { t.Id })
            .ToListAsync();

        foreach (var t in tenants)
        {
            var existingCount = await context.Departments
                .IgnoreQueryFilters()
                .CountAsync(d => d.TenantId == t.Id);
            if (existingCount > 0)
                continue;

            foreach (var (name, description) in DefaultDepartments)
            {
                var exists = await context.Departments
                    .IgnoreQueryFilters()
                    .AnyAsync(d => d.TenantId == t.Id && d.Name == name);
                if (exists)
                    continue;

                await context.Departments.AddAsync(new Department
                {
                    Name = name,
                    Description = description,
                    IsActive = true,
                    TenantId = t.Id
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
