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
        await SeedGlobalDemoRolePermissions(dbContext);
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
        tenantAdminPermissions.Add(MenuPermissionConstant.ProfileView);

        // Budget Management (23-xx) - full access for TenantAdmin
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.Budget.GetAllValues());
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.Department.GetAllValues());
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.ApprovalConfig.GetAllValues());
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.BudgetReport.GetAllValues());
        tenantAdminPermissions.Add(MenuPermissionConstant.BudgetRequestView);
        tenantAdminPermissions.Add(MenuPermissionConstant.BudgetRequestCreate);
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
        tenantAdminPermissions.AddRange(MenuPermissionDefinitions.NepaliFiscalYear.GetAllValues());

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

    /// <summary>Seeds role-specific permissions for global Admin, CEO, CFO, HOD (no-tenant demo roles). Dashboard and menu access differ per role.</summary>
    private static async Task SeedGlobalDemoRolePermissions(ApplicationDataContext dbContext)
    {
        // Admin (global) – full access (already seeded by SeedTenantAdminPermissions; ensure global Admin has permissions)
        var adminRoleId = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == null && r.Name == SystemRoles.Admin)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
        if (!string.IsNullOrEmpty(adminRoleId))
            await SetRolePermissionsAsync(dbContext, adminRoleId, GetAdminPermissions());

        // CEO – high-level: dashboard, notifications, operations, budget report view/export, approve/reject requests, memo view only
        var ceoRoleId = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == null && r.Name == SystemRoles.CEO)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
        if (!string.IsNullOrEmpty(ceoRoleId))
            await SetRolePermissionsAsync(dbContext, ceoRoleId, GetCEOPermissions());

        // CFO – finance: dashboard, full budget & report, approval config view, memo full, department view, signatures
        var cfoRoleId = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == null && r.Name == SystemRoles.CFO)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
        if (!string.IsNullOrEmpty(cfoRoleId))
            await SetRolePermissionsAsync(dbContext, cfoRoleId, GetCFOPermissions());

        // HOD – department: dashboard, department view, memo view/create, budget report view, budget headings view, budget request view/create
        var hodRoleId = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == null && r.Name == SystemRoles.HOD)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
        if (!string.IsNullOrEmpty(hodRoleId))
            await SetRolePermissionsAsync(dbContext, hodRoleId, GetHODPermissions());

        await dbContext.SaveChangesAsync();
    }

    private static async Task SetRolePermissionsAsync(ApplicationDataContext dbContext, string roleId, List<string> permissions)
    {
        var roleClaim = await dbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == roleId);
        if (roleClaim == null)
        {
            await dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = permissions
            });
        }
        else
        {
            roleClaim.Permissions = permissions.Distinct().ToList();
            dbContext.RoleClaims.Update(roleClaim);
        }
    }

    private static List<string> GetAdminPermissions()
    {
        var p = new List<string>
        {
            MenuPermissionConstant.DashboardView,
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.SystemView,
            MenuPermissionConstant.LogsView,
            MenuPermissionConstant.SystemLogView,
            MenuPermissionConstant.ConfigView,
            MenuPermissionConstant.ProfileView
        };
        p.AddRange(MenuPermissionDefinitions.Budget.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Department.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.ApprovalConfig.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.BudgetReport.GetAllValues());
        p.Add(MenuPermissionConstant.BudgetRequestView);
        p.Add(MenuPermissionConstant.BudgetRequestCreate);
        p.Add(MenuPermissionConstant.BudgetRequestApprove);
        p.Add(MenuPermissionConstant.BudgetRequestReject);
        p.Add(MenuPermissionConstant.MemoView);
        p.Add(MenuPermissionConstant.MemoCreate);
        p.Add(MenuPermissionConstant.MemoUpdate);
        p.Add(MenuPermissionConstant.MemoDelete);
        p.Add(MenuPermissionConstant.MemoExport);
        p.Add(MenuPermissionConstant.MemoGeneratePdf);
        p.Add(MenuPermissionConstant.SignatureView);
        p.Add(MenuPermissionConstant.SignatureUpload);
        p.AddRange(MenuPermissionDefinitions.BudgetHeadings.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.NepaliFiscalYear.GetAllValues());
        return p.Distinct().ToList();
    }

    private static List<string> GetCEOPermissions()
    {
        return new List<string>
        {
            MenuPermissionConstant.DashboardView,
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.BudgetReportView,
            MenuPermissionConstant.BudgetReportExport,
            MenuPermissionConstant.BudgetRequestView,
            MenuPermissionConstant.BudgetRequestApprove,
            MenuPermissionConstant.BudgetRequestReject,
            MenuPermissionConstant.MemoView,
            MenuPermissionConstant.ProfileView
        }.Distinct().ToList();
    }

    private static List<string> GetCFOPermissions()
    {
        var p = new List<string>
        {
            MenuPermissionConstant.DashboardView,
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.BudgetReportView,
            MenuPermissionConstant.BudgetReportExport,
            MenuPermissionConstant.BudgetRequestView,
            MenuPermissionConstant.BudgetRequestApprove,
            MenuPermissionConstant.BudgetRequestReject,
            MenuPermissionConstant.ApprovalConfigView,
            MenuPermissionConstant.SignatureView,
            MenuPermissionConstant.SignatureUpload,
            MenuPermissionConstant.DepartmentView,
            MenuPermissionConstant.ProfileView
        };
        p.AddRange(MenuPermissionDefinitions.Budget.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.BudgetReport.GetAllValues());
        p.Add(MenuPermissionConstant.MemoView);
        p.Add(MenuPermissionConstant.MemoCreate);
        p.Add(MenuPermissionConstant.MemoUpdate);
        p.Add(MenuPermissionConstant.MemoDelete);
        p.Add(MenuPermissionConstant.MemoExport);
        p.Add(MenuPermissionConstant.MemoGeneratePdf);
        p.Add(MenuPermissionConstant.BudgetHeadingsView);
        return p.Distinct().ToList();
    }

    private static List<string> GetHODPermissions()
    {
        // HOD is department-scoped; they should not manage departments or budget headings from the menu.
        // Exclude DepartmentView and BudgetHeadingsView so those modules are hidden in the UI for HOD.
        return new List<string>
        {
            MenuPermissionConstant.DashboardView,
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.BudgetReportView,
            MenuPermissionConstant.BudgetRequestView,
            MenuPermissionConstant.BudgetRequestCreate,
            MenuPermissionConstant.MemoView,
            MenuPermissionConstant.MemoCreate,
            MenuPermissionConstant.MemoGeneratePdf,
            MenuPermissionConstant.ProfileView
        }.Distinct().ToList();
    }
}
