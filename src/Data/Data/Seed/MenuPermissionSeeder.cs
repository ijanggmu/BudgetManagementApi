using Data.Context;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constant.Roles;
using static SharedKernel.Constant.Permission.MenuPermissionsList;

namespace Data.Seed;
public static class MenuPermissionSeeder
{
    public async static Task SeedPermissionsForRole(ApplicationDataContext dbContext)
    {
        await SeedSuperAdminPermissions(dbContext);
    }

    private static async Task SeedSuperAdminPermissions(ApplicationDataContext dbContext)
    {
        var roleId = await dbContext.Roles.Where(userRole => userRole.Name == SystemRoles.SuperAdmin)
                                          .Select(y => y.Id).FirstOrDefaultAsync();

        var roleClaim = await dbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == roleId);
        var permissions = MenuManager.GetAllPermissions();

        if (roleClaim == null)
        {
            await dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = permissions.Select(x => x.Value).ToList()
            });
        }
        else
        {
            roleClaim.Permissions = permissions.Select(x => x.Value).ToList();
            dbContext.RoleClaims.Update(roleClaim);
        }

        await dbContext.SaveChangesAsync();

    }
}
