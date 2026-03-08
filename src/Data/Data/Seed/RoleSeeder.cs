using Data.Context;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constant.Roles;

namespace Data.Seed;

public static class RoleSeeder
{
    /// <summary>Seeds global/system roles (SuperAdmin, Admin).</summary>
    public static async Task SeedData(RoleManager<ApplicationRole> roleManager)
    {
        var roleList = new List<ApplicationRole>
        {
            new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = SystemRoles.SuperAdmin,
                RoleDisplayName = "Super Admin",
                Description = SystemRoles.SuperAdmin,
                RoleLevel = SystemRoles.SuperAdminLevel,
                RoleType = SystemRoles.SuperAdmin
            },
            new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = SystemRoles.Admin,
                RoleDisplayName = "Administrator",
                Description = SystemRoles.Admin,
                RoleLevel = SystemRoles.AdminLevel,
                RoleType = SystemRoles.Admin
            }
        };

        foreach (var role in roleList)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    /// <summary>Seeds global roles (CEO, CFO, HOD) with no tenant for demo login without creating a tenant.</summary>
    public static async Task SeedGlobalDemoRoles(ApplicationDataContext context, RoleManager<ApplicationRole> roleManager)
    {
        var globalDemoRoles = new[]
        {
            (Name: SystemRoles.CEO, DisplayName: "Chief Executive Officer"),
            (Name: SystemRoles.CFO, DisplayName: "Chief Financial Officer"),
            (Name: SystemRoles.HOD, DisplayName: "Head of Department")
        };

        foreach (var (name, displayName) in globalDemoRoles)
        {
            var normalizedName = roleManager.NormalizeKey(name);
            var exists = await context.Roles
                .IgnoreQueryFilters()
                .AnyAsync(r => r.TenantId == null && r.NormalizedName == normalizedName);
            if (exists)
                continue;

            var role = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                NormalizedName = normalizedName,
                RoleDisplayName = displayName,
                Description = name,
                RoleLevel = SystemRoles.CEOCFOHODLevel,
                RoleType = name
                // TenantId left null for global demo role
            };
            await roleManager.CreateAsync(role);
        }
        await context.SaveChangesAsync();
    }

    /// <summary>Seeds tenant-specific roles (CEO, CFO, HOD) for each tenant.</summary>
    public static async Task SeedTenantRoles(ApplicationDataContext context, RoleManager<ApplicationRole> roleManager)
    {
        var tenants = await context.Tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new { t.Id, t.Name })
            .ToListAsync();

        var tenantRoles = SystemRoles.GetTenantDefaultRoles();

        foreach (var tenant in tenants)
        {
            foreach (var roleName in tenantRoles)
            {
                var normalizedName = roleManager.NormalizeKey(roleName);
                var exists = await context.Roles
                    .IgnoreQueryFilters()
                    .AnyAsync(r => r.TenantId == tenant.Id && r.NormalizedName == normalizedName);

                if (exists)
                    continue;

                var role = new ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = normalizedName,
                    RoleDisplayName = GetRoleDisplayName(roleName),
                    Description = roleName,
                    RoleLevel = SystemRoles.CEOCFOHODLevel,
                    RoleType = roleName,
                    TenantId = tenant.Id
                };

                await roleManager.CreateAsync(role);
            }
        }

        await context.SaveChangesAsync();
    }

    private static string GetRoleDisplayName(string roleName)
    {
        return roleName switch
        {
            SystemRoles.CEO => "Chief Executive Officer",
            SystemRoles.CFO => "Chief Financial Officer",
            SystemRoles.HOD => "Head of Department",
            _ => roleName
        };
    }
}

