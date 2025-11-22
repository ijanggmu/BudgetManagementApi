using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.AgentEntity;
using Data.Entities.CorporateEntity;
using Data.Entities.CustomerEntity;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constant.Roles;

namespace Data.Seed;
public static class UserSeeder
{
    public static async Task SeedData(ApplicationDataContext context,
                                  UserManager<ApplicationUser> userManager)
    {
        var usersToSeed = new[]
        {
        new
        {
            UserName = "superadmin",
            Email = "superadmin@hei.com",
            Role = SystemRoles.SuperAdmin,
            FullName = "Super Admin",
            EntityType = "Admin"
        },
        new
        {
            UserName = "9999999999",
            Email = "supercustomer@hei.com",
            Role = SystemRoles.Individual,
            FullName = "Super Customer",
            EntityType = "Customer"
        },
        new
        {
            UserName = "superagent",
            Email = "superagent@hei.com",
            Role = SystemRoles.Agent,
            FullName = "Super Agent",
            EntityType = "Agent"
        },
        new
        {
            UserName = "supercorporate",
            Email = "supercorportae@hei.com",
            Role = SystemRoles.Corporate,
            FullName = "Super Corporate",
            EntityType = "Corporate"
        }
    };

        foreach (var userInfo in usersToSeed)
        {
            var existingUser = await userManager.FindByNameAsync(userInfo.UserName);
            if (existingUser != null)
                continue;

            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userInfo.UserName,
                Email = userInfo.Email,
                IsDisabled = false,
                LockoutEnabled = false,
                PhoneNumberConfirmed = true,
                // SuperAdmin should have null TenantId to access all tenants
                // Other users will get TenantId assigned automatically when they're created in tenant context
                TenantId = userInfo.Role == SystemRoles.SuperAdmin ? null : null // Will be set by context if needed
            };

            // TODO: Move password to secure storage or secrets
            var result = await userManager.CreateAsync(newUser, "Admin@123");
            if (!result.Succeeded)
                throw new Exception($"Failed to create user {userInfo.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(newUser, userInfo.Role);

            switch (userInfo.EntityType)
            {
                case "Admin":
                    await context.Admins.AddAsync(new Admin
                    {
                        FullName = userInfo.FullName,
                        UserId = newUser.Id
                    });
                    break;
                case "Customer":
                    await context.Customers.AddAsync(new Customer
                    {
                        FullName = userInfo.FullName,
                        UserId = newUser.Id
                    });
                    break;
                case "Agent":
                    await context.Agents.AddAsync(new Agent
                    {
                        FullName = userInfo.FullName,
                        UserId = newUser.Id
                    });
                    break;
                case "Corporate":
                    await context.Corporates.AddAsync(new Corporate
                    {
                        CorporateName = userInfo.FullName,
                        UserId = newUser.Id
                    });
                    break;
            }

            await context.SaveChangesAsync();
        }
    }

}
