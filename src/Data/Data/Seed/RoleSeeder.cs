using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Constant.Roles;
namespace Data.Seed;
public static class RoleSeeder
{
    public static async Task SeedData(RoleManager<ApplicationRole> roleManager)
    {
        var roleList = new List<ApplicationRole>
                            {
                                new ApplicationRole
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = SystemRoles.SuperAdmin,
                                    Description = SystemRoles.SuperAdmin,
                                    RoleLevel = SystemRoles.SuperAdminLevel,
                                    RoleType = SystemRoles.SuperAdmin
                                },
                                new ApplicationRole
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = SystemRoles.Admin,
                                    Description = SystemRoles.Admin,
                                    RoleLevel = SystemRoles.AdminLevel,
                                    RoleType = SystemRoles.Admin
                                },
                                new ApplicationRole
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = SystemRoles.FoDo,
                                    Description = SystemRoles.FoDo,
                                    RoleLevel = SystemRoles.FoDoLevel,
                                    RoleType = SystemRoles.FoDo
                                },
                                new ApplicationRole
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = SystemRoles.MarketingExecutive,
                                    Description = SystemRoles.MarketingExecutive,
                                    RoleLevel = SystemRoles.MarketingExecutiveLevel,
                                    RoleType = SystemRoles.MarketingExecutive
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
}

