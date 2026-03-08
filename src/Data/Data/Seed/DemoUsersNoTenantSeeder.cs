using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constant.Roles;

namespace Data.Seed;

/// <summary>
/// Seeds 4 demo accounts (Admin, CEO, CFO, HOD) with no tenant for simulating all roles in admin without creating a tenant.
/// All use password: Admin@123
/// </summary>
public static class DemoUsersNoTenantSeeder
{
    public const string DefaultPassword = "Admin@123";

    private static readonly (string UserName, string Email, string FullName, string RoleName)[] Users = new[]
    {
        ("admin", "admin@demo.local", "Demo Admin", SystemRoles.Admin),
        ("ceo", "ceo@demo.local", "Demo CEO", SystemRoles.CEO),
        ("cfo", "cfo@demo.local", "Demo CFO", SystemRoles.CFO),
        ("hod", "hod@demo.local", "Demo HOD", SystemRoles.HOD)
    };

    public static async Task SeedAsync(
        ApplicationDataContext context,
        UserManager<ApplicationUser> userManager)
    {
        var globalRoleIds = await context.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == null && Users.Select(u => u.RoleName).Contains(r.Name))
            .ToDictionaryAsync(r => r.Name, r => r.Id);

        foreach (var u in Users)
        {
            if (!globalRoleIds.TryGetValue(u.RoleName, out var roleId))
                continue;

            var existing = await context.Users
                .IgnoreQueryFilters()
                .AnyAsync(usr => usr.TenantId == null && usr.UserName == u.UserName);
            if (existing)
                continue;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = u.UserName,
                Email = u.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsDisabled = false,
                TenantId = null
            };

            var createResult = await userManager.CreateAsync(user, DefaultPassword);
            if (!createResult.Succeeded)
                throw new InvalidOperationException($"Failed to create user {u.UserName}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            context.UserRoles.Add(new ApplicationUserRoles
            {
                UserId = user.Id,
                RoleId = roleId,
                CreatedOn = DateTime.UtcNow
            });

            await context.Admins.AddAsync(new Admin
            {
                FullName = u.FullName,
                UserId = user.Id,
                TenantId = null
            });

            await context.SaveChangesAsync();
        }
    }
}
