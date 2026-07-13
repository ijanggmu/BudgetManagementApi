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
        await SeedPerTenantAdminRolePermissions(dbContext);
        await SeedPerTenantBusinessRolePermissions(dbContext);
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
        var tenantAdminPermissions = GetAdminPermissions();

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

        var hodAssistRoleId = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == null && r.Name == SystemRoles.HodAssistance)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
        if (!string.IsNullOrEmpty(hodAssistRoleId))
            await SetRolePermissionsAsync(dbContext, hodAssistRoleId, GetHodAssistancePermissions());

        await dbContext.SaveChangesAsync();
    }

    /// <summary>Ensures menu claims exist for every tenant-scoped HodAssistance role (for existing DBs after the role is added).</summary>
    public static async Task EnsureTenantHodAssistanceRoleMenuClaimsAsync(ApplicationDataContext dbContext, CancellationToken cancellationToken = default)
    {
        var roleIds = await dbContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.TenantId != null && r.RoleType == SystemRoles.HodAssistance)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        foreach (var roleId in roleIds)
        {
            await UpsertRoleMenuPermissionsAsync(dbContext, roleId, GetHodAssistancePermissions(), cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Insert or update <see cref="ApplicationRoleClaim.Permissions"/> for a role (no SaveChanges).</summary>
    public static async Task UpsertRoleMenuPermissionsAsync(
        ApplicationDataContext dbContext,
        string roleId,
        IReadOnlyList<string> permissions,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(roleId))
            return;

        var distinct = permissions.Distinct().ToList();
        var roleClaim = await dbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == roleId, cancellationToken);
        if (roleClaim == null)
        {
            await dbContext.RoleClaims.AddAsync(
                new ApplicationRoleClaim { RoleId = roleId, Permissions = distinct },
                cancellationToken);
        }
        else
        {
            roleClaim.Permissions = distinct;
            dbContext.RoleClaims.Update(roleClaim);
        }
    }

    private static Task SetRolePermissionsAsync(ApplicationDataContext dbContext, string roleId, List<string> permissions) =>
        UpsertRoleMenuPermissionsAsync(dbContext, roleId, permissions);

    /// <summary>Full tenant-admin permission set (all tenant features except SuperAdmin-only Tenants).</summary>
    public static List<string> GetAdminPermissions()
    {
        var p = new List<string>
        {
            MenuPermissionConstant.DashboardView,
            MenuPermissionConstant.AdministrationView,
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.SystemView,
            MenuPermissionConstant.LogsView,
            MenuPermissionConstant.SystemLogView,
            MenuPermissionConstant.ConfigView,
        };

        // Dashboard utilities (profile, password, 2FA, uploads, common APIs)
        p.AddRange(MenuPermissionDefinitions.Profile.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Password.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.TwoFactor.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.CommonUtilities.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.FileUpload.GetAllValues());

        // Administration (tenant-scoped; Tenants menu is SuperAdmin-only)
        p.AddRange(MenuPermissionDefinitions.Branding.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Branch.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Designation.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Roles.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Menu.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.AdminManagement.GetAllValues());
        p.AddRange(MenuPermissionDefinitions.Entity.GetAllValues());

        // Budget management
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

    /// <summary>Sync tenant-scoped Admin roles (e.g. Admin-acme) with the current admin permission set.</summary>
    private static async Task SeedPerTenantAdminRolePermissions(ApplicationDataContext dbContext)
    {
        var tenantAdminRoleIds = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => !r.IsDeleted && r.TenantId != null && r.RoleType == SystemRoles.Admin)
            .Select(r => r.Id)
            .ToListAsync();

        foreach (var roleId in tenantAdminRoleIds)
            await SetRolePermissionsAsync(dbContext, roleId, GetAdminPermissions());

        if (tenantAdminRoleIds.Count > 0)
            await dbContext.SaveChangesAsync();
    }

    /// <summary>Sync CEO/CFO/HOD/HodAssistance tenant roles (e.g. CEO-hei) with current permission sets.</summary>
    private static async Task SeedPerTenantBusinessRolePermissions(ApplicationDataContext dbContext)
    {
        var businessRoles = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(r => !r.IsDeleted && r.TenantId != null &&
                        (r.RoleType == SystemRoles.CEO ||
                         r.RoleType == SystemRoles.CFO ||
                         r.RoleType == SystemRoles.HOD ||
                         r.RoleType == SystemRoles.HodAssistance))
            .Select(r => new { r.Id, r.RoleType })
            .ToListAsync();

        foreach (var role in businessRoles)
        {
            var perms = role.RoleType switch
            {
                SystemRoles.CEO => GetCEOPermissions(),
                SystemRoles.CFO => GetCFOPermissions(),
                SystemRoles.HOD => GetHODPermissions(),
                SystemRoles.HodAssistance => GetHodAssistancePermissions(),
                _ => new List<string>()
            };
            if (perms.Count > 0)
                await SetRolePermissionsAsync(dbContext, role.Id, perms);
        }

        if (businessRoles.Count > 0)
            await dbContext.SaveChangesAsync();
    }

    /// <summary>Profile, auth utilities, and uploads required by the app shell for every portal role.</summary>
    private static void AddPortalBasePermissions(List<string> permissions)
    {
        permissions.Add(MenuPermissionConstant.DashboardView);
        permissions.AddRange(MenuPermissionDefinitions.Profile.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.Password.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.TwoFactor.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.CommonUtilities.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.FileUpload.GetAllValues());
    }

    public static List<string> GetCEOPermissions()
    {
        var p = new List<string>
        {
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.BudgetReportView,
            MenuPermissionConstant.BudgetReportExport,
            MenuPermissionConstant.BudgetRequestView,
            MenuPermissionConstant.BudgetRequestApprove,
            MenuPermissionConstant.BudgetRequestReject,
            MenuPermissionConstant.MemoView,
            MenuPermissionConstant.MemoCreate,
            MenuPermissionConstant.MemoUpdate,
            MenuPermissionConstant.MemoExport,
            MenuPermissionConstant.MemoGeneratePdf,
        };
        AddPortalBasePermissions(p);
        return p.Distinct().ToList();
    }

    public static List<string> GetCFOPermissions()
    {
        var p = new List<string>
        {
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
        AddPortalBasePermissions(p);
        return p.Distinct().ToList();
    }

    public static List<string> GetHODPermissions()
    {
        // HOD is department-scoped; they should not manage departments or budget headings from the menu.
        // Exclude DepartmentView and BudgetHeadingsView so those modules are hidden in the UI for HOD.
        var p = new List<string>
        {
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.BudgetReportView,
            MenuPermissionConstant.BudgetRequestView,
            MenuPermissionConstant.BudgetRequestCreate,
            MenuPermissionConstant.BudgetRequestApprove,
            MenuPermissionConstant.BudgetRequestReject,
            MenuPermissionConstant.MemoView,
            MenuPermissionConstant.MemoCreate,
            MenuPermissionConstant.MemoUpdate,
            MenuPermissionConstant.MemoExport,
            MenuPermissionConstant.MemoGeneratePdf,
        };
        AddPortalBasePermissions(p);
        return p.Distinct().ToList();
    }

    /// <summary>HOD assistant: prepare memos and budget requests for their department only (no approval chain).</summary>
    public static List<string> GetHodAssistancePermissions()
    {
        var p = new List<string>
        {
            MenuPermissionConstant.BudgetManagementView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.BudgetReportView,
            MenuPermissionConstant.BudgetRequestView,
            MenuPermissionConstant.BudgetRequestCreate,
            MenuPermissionConstant.MemoView,
            MenuPermissionConstant.MemoCreate,
            MenuPermissionConstant.MemoUpdate,
            MenuPermissionConstant.MemoGeneratePdf,
        };
        AddPortalBasePermissions(p);
        return p.Distinct().ToList();
    }
}
