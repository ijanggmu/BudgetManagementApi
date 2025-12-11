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
        await SeedFoDoPermissions(dbContext);
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

        // Tenant Admin permissions: Sales & Marketing, Operations, System sections with full CRUD+Export
        var tenantAdminPermissions = new List<string>
        {
            // Sales & Marketing section - Full CRUD+Export
            MenuPermissionConstant.SalesMarketingView
        };

        // Add all CRUD+Export permissions for Marketing Executives
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.MarketingExecutives.GetAllValues());

        // Add all CRUD+Export permissions for Admin Leads
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.AdminLeads.GetAllValues());

        // Add all CRUD+Export permissions for Admin Quotations
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.AdminQuotations.GetAllValues());

        // Operations section
        tenantAdminPermissions.Add(MenuPermissionConstant.OperationsView);
        tenantAdminPermissions.Add(MenuPermissionConstant.NotificationsView);

        // System section
        tenantAdminPermissions.Add(MenuPermissionConstant.SystemView);
        tenantAdminPermissions.Add(MenuPermissionConstant.LogsView);
        tenantAdminPermissions.Add(MenuPermissionConstant.SystemLogView);
        tenantAdminPermissions.Add(MenuPermissionConstant.ConfigView);
        tenantAdminPermissions.Add(MenuPermissionConstant.MenuView);
        tenantAdminPermissions.Add(MenuPermissionConstant.MenuUpdate);

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

    private static async Task SeedFoDoPermissions(ApplicationDataContext dbContext)
    {
        var roleId = await dbContext.Roles.Where(userRole => userRole.Name == SystemRoles.FoDo)
                                          .Select(y => y.Id).FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(roleId))
            return;

        var roleClaim = await dbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == roleId);

        // FoDo permissions: Only Sales & Marketing section (NO admin access)
        // FoDo can manage their own leads and quotations through FoDo endpoints
        // They CANNOT access Admin endpoints (AdminLeadController, AdminQuotationController, etc.)
        // because those require AdminLeadsView/AdminQuotationsView permissions which FoDo doesn't have
        var fodoPermissions = new List<string>
        {
            // Sales & Marketing section - View only (parent menu)
            MenuPermissionConstant.SalesMarketingView,
            
            // Note: FoDo endpoints (FoDo/Lead, FoDo/Quotation) don't require specific permissions
            // They are protected by role-based authorization (BaseFoDoApiController)
            // FoDo users can only access their own data through FoDo endpoints, not Admin endpoints
        };

        fodoPermissions = fodoPermissions.Distinct().ToList();

        if (roleClaim == null)
        {
            await dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = fodoPermissions
            });
        }
        else
        {
            // Merge with existing permissions, avoiding duplicates
            var existingPermissions = roleClaim.Permissions ?? new List<string>();
            var mergedPermissions = existingPermissions.Union(fodoPermissions).Distinct().ToList();
            roleClaim.Permissions = mergedPermissions;
            dbContext.RoleClaims.Update(roleClaim);
        }

        await dbContext.SaveChangesAsync();
    }
}
