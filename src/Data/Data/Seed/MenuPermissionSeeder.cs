using Data.Context;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constant.Roles;
using SharedKernel.Constant.Permission;
using static SharedKernel.Constant.Permission.MenuPermissionsList;

namespace Data.Seed;

public static class MenuPermissionSeeder
{
    public async static Task SeedPermissionsForRole(ApplicationDataContext dbContext)
    {
        await SeedSuperAdminPermissions(dbContext);
        await SeedTenantAdminPermissions(dbContext);
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

    private static async Task SeedTenantAdminPermissions(ApplicationDataContext dbContext)
    {
        var roleId = await dbContext.Roles.Where(userRole => userRole.Name == SystemRoles.Admin)
                                          .Select(y => y.Id).FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(roleId))
            return;

        var roleClaim = await dbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == roleId);

        // Tenant Admin permissions: Operations, System sections with full CRUD+Export
        var tenantAdminPermissions = new List<string>();

        // Operations section
        tenantAdminPermissions.Add(MenuPermissionConstant.OperationsView);
        tenantAdminPermissions.Add(MenuPermissionConstant.NotificationsView);

        // System section
        tenantAdminPermissions.Add(MenuPermissionConstant.SystemView);
        tenantAdminPermissions.Add(MenuPermissionConstant.LogsView);
        tenantAdminPermissions.Add(MenuPermissionConstant.SystemLogView);
        tenantAdminPermissions.Add(MenuPermissionConstant.ConfigView);
        tenantAdminPermissions.Add(MenuPermissionConstant.DashboardView);

        // Budget Management (23-xx) - full access for TenantAdmin
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.Budget.GetAllValues());
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.Department.GetAllValues());
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.ApprovalConfig.GetAllValues());
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.BudgetReport.GetAllValues());
        tenantAdminPermissions.Add(MenuPermissionConstant.BudgetRequestApprove);
        tenantAdminPermissions.Add(MenuPermissionConstant.BudgetRequestReject);
        tenantAdminPermissions.Add(MenuPermissionConstant.MemoView);
        tenantAdminPermissions.Add(MenuPermissionConstant.MemoCreate);
        tenantAdminPermissions.Add(MenuPermissionConstant.MemoUpdate);
        tenantAdminPermissions.Add(MenuPermissionConstant.MemoDelete);
        tenantAdminPermissions.Add(MenuPermissionConstant.MemoExport);
        tenantAdminPermissions.Add(MenuPermissionConstant.MemoGeneratePdf);
        tenantAdminPermissions.Add(MenuPermissionConstant.SignatureView);
        tenantAdminPermissions.Add(MenuPermissionConstant.SignatureUpload);
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.BudgetHeadings.GetAllValues());

        tenantAdminPermissions = tenantAdminPermissions.Distinct().ToList();

        if (roleClaim == null)
        {
            await dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = tenantAdminPermissions
            });
        }
        else
        {
            // Merge with existing permissions, avoiding duplicates
            var existingPermissions = roleClaim.Permissions ?? new List<string>();
            var mergedPermissions = existingPermissions.Union(tenantAdminPermissions).Distinct().ToList();
            roleClaim.Permissions = mergedPermissions;
            dbContext.RoleClaims.Update(roleClaim);
        }

        await dbContext.SaveChangesAsync();
    }
}
