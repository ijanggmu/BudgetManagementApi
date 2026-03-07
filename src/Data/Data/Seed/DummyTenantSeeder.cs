using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constant.Permission;
using SharedKernel.Constant.Roles;

namespace Data.Seed;

/// <summary>
/// Seeds one dummy tenant with 4 users: Tenant Admin, CEO, CFO, HOD.
/// All users share the same password: Admin@123
/// </summary>
public static class DummyTenantSeeder
{
    public const string DummyTenantSlug = "dummy";
    public const string DummyTenantName = "Dummy Tenant";
    public const string DefaultPassword = "Admin@123";

    public static async Task SeedAsync(
        ApplicationDataContext context,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Slug == DummyTenantSlug && !t.IsDeleted);
        if (tenant == null)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                tenant = new Tenant
                {
                    Name = DummyTenantName,
                    Slug = DummyTenantSlug,
                    IsActive = true
                };
                await context.Tenants.AddAsync(tenant);
                await context.SaveChangesAsync();

                var branding = new CompanyBranding
                {
                    TenantId = tenant.Id,
                    LogoUrl = "",
                    PaletteJson = "{}",
                    TypographyJson = "{}",
                    Version = 1
                };
                await context.CompanyBrandings.AddAsync(branding);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        var adminRoleName = $"{SystemRoles.Admin}-{DummyTenantSlug}";

        await CreateRoleIfNotExistsAsync(context, roleManager, tenant.Id, adminRoleName, SystemRoles.Admin, SystemRoles.AdminLevel, "Tenant Administrator");
        await CreateRoleIfNotExistsAsync(context, roleManager, tenant.Id, SystemRoles.CEO, SystemRoles.CEO, SystemRoles.CEOCFOHODLevel, "Chief Executive Officer");
        await CreateRoleIfNotExistsAsync(context, roleManager, tenant.Id, SystemRoles.CFO, SystemRoles.CFO, SystemRoles.CEOCFOHODLevel, "Chief Financial Officer");
        await CreateRoleIfNotExistsAsync(context, roleManager, tenant.Id, SystemRoles.HOD, SystemRoles.HOD, SystemRoles.CEOCFOHODLevel, "Head of Department");
        await context.SaveChangesAsync();

        await SeedAdminRolePermissionsAsync(context, tenant.Id, adminRoleName);

        var roleIdsByName = await context.Roles
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == tenant.Id)
            .ToDictionaryAsync(r => r.Name, r => r.Id);

        var usersToSeed = new[]
        {
            (UserName: "tenantadmin", Email: "tenantadmin@dummy.com", FullName: "Tenant Admin", RoleName: adminRoleName, IsAdminEntity: true),
            (UserName: "ceo", Email: "ceo@dummy.com", FullName: "CEO User", RoleName: SystemRoles.CEO, IsAdminEntity: false),
            (UserName: "cfo", Email: "cfo@dummy.com", FullName: "CFO User", RoleName: SystemRoles.CFO, IsAdminEntity: false),
            (UserName: "hod", Email: "hod@dummy.com", FullName: "HOD User", RoleName: SystemRoles.HOD, IsAdminEntity: false)
        };

        foreach (var u in usersToSeed)
        {
            var existingForTenant = await context.Users
                .IgnoreQueryFilters()
                .AnyAsync(usr => usr.TenantId == tenant.Id && usr.UserName == u.UserName);
            if (existingForTenant)
                continue;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = u.UserName,
                Email = u.Email,
                EmailConfirmed = true,
                IsDisabled = false,
                TenantId = tenant.Id
            };
            var createResult = await userManager.CreateAsync(user, DefaultPassword);
            if (!createResult.Succeeded)
                throw new InvalidOperationException($"Failed to create user {u.UserName}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            if (!roleIdsByName.TryGetValue(u.RoleName, out var roleId))
                throw new InvalidOperationException($"Role {u.RoleName} not found for tenant {tenant.Id}");
            context.UserRoles.Add(new ApplicationUserRoles
            {
                UserId = user.Id,
                RoleId = roleId,
                CreatedOn = DateTime.UtcNow
            });

            if (u.IsAdminEntity)
            {
                await context.Admins.AddAsync(new Admin
                {
                    FullName = u.FullName,
                    UserId = user.Id
                });
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task CreateRoleIfNotExistsAsync(
        ApplicationDataContext context,
        RoleManager<ApplicationRole> roleManager,
        string tenantId,
        string roleName,
        string roleType,
        int roleLevel,
        string roleDisplayName = null)
    {
        var normalizedName = roleManager.NormalizeKey(roleName);
        if (await context.Roles.IgnoreQueryFilters().AnyAsync(r => r.TenantId == tenantId && r.NormalizedName == normalizedName))
            return;

        var role = new ApplicationRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = roleName,
            NormalizedName = normalizedName,
            RoleDisplayName = roleDisplayName ?? roleName,
            Description = roleName,
            RoleLevel = roleLevel,
            RoleType = roleType,
            TenantId = tenantId
        };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    private static async Task SeedAdminRolePermissionsAsync(ApplicationDataContext context, string tenantId, string adminRoleName)
    {
        var roleId = await context.Roles
            .IgnoreQueryFilters()
            .Where(r => r.Name == adminRoleName && r.TenantId == tenantId)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
        if (string.IsNullOrEmpty(roleId))
            return;

        var permissions = new List<string>
        {
            MenuPermissionConstant.SalesMarketingView,
            MenuPermissionConstant.OperationsView,
            MenuPermissionConstant.NotificationsView,
            MenuPermissionConstant.SystemView,
            MenuPermissionConstant.LogsView,
            MenuPermissionConstant.SystemLogView,
            MenuPermissionConstant.ConfigView,
            MenuPermissionConstant.DashboardView
        };
        permissions.AddRange(MenuPermissionDefinitions.MarketingExecutives.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.AdminLeads.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.AdminQuotations.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.Budget.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.Department.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.ApprovalConfig.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.BudgetReport.GetAllValues());
        permissions.AddRange(MenuPermissionDefinitions.BudgetHeadings.GetAllValues());
        permissions.Add(MenuPermissionConstant.BudgetRequestApprove);
        permissions.Add(MenuPermissionConstant.BudgetRequestReject);
        permissions.Add(MenuPermissionConstant.MemoView);
        permissions.Add(MenuPermissionConstant.MemoCreate);
        permissions.Add(MenuPermissionConstant.MemoUpdate);
        permissions.Add(MenuPermissionConstant.MemoDelete);
        permissions.Add(MenuPermissionConstant.MemoExport);
        permissions.Add(MenuPermissionConstant.MemoGeneratePdf);
        permissions.Add(MenuPermissionConstant.SignatureView);
        permissions.Add(MenuPermissionConstant.SignatureUpload);
        permissions = permissions.Distinct().ToList();

        var roleClaim = await context.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == roleId);
        if (roleClaim == null)
        {
            await context.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = permissions
            });
        }
        else
        {
            roleClaim.Permissions = (roleClaim.Permissions ?? new List<string>()).Union(permissions).Distinct().ToList();
            context.RoleClaims.Update(roleClaim);
        }
        await context.SaveChangesAsync();
    }
}
