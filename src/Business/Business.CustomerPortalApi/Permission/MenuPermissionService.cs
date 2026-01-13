using System;
using System.Collections.Immutable;
using System.Threading;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Roles;
using Models.Common;
using Models.Common.Menu;
using SharedKernel.Constant.Permission;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static SharedKernel.Constant.Permission.MenuPermissionsList;

namespace Business.BeemaEdgeApi.Permission;

public class MenuPermissionService(ApplicationDataContext context,
    IUserProfileService personAccessor) : IMenuPermissionService
{
    public Result<MenuModel> GetMenu()
    {
        var roleId = personAccessor.GetRoleId();
        var roleType = personAccessor.GetRoleType();

        if (string.IsNullOrEmpty(roleId))
            return Result<MenuModel>.Success(new MenuModel());

        var roleNameList = roleId.Split(",");

        var permissionList = (from role in context.Roles
                              join roleClaim in context.RoleClaims
                              on role.Id equals roleClaim.RoleId
                              where roleNameList.Contains(role.Name) && !role.IsDeleted
                              select roleClaim.Permissions)
                          .ToList();

        var permissions = permissionList.SelectMany(x => x).ToList();

        if (!permissions.Any())
            return Result<MenuModel>.Success(new MenuModel());

        var menus = MenuManager.GetMenusForPermissions(permissions, [roleType]);

        return Result<MenuModel>.Success(new MenuModel { MenuList = menus });

    }

    public Result<RolePermissionViewModel> GetAllMenuByRoleId(string roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            return Result<RolePermissionViewModel>.Failed("RoleId is required.");

        // Get current user's role to check for SuperAdmin
        var currentRole = personAccessor.GetRoleId();
        var isSuperAdmin = !string.IsNullOrWhiteSpace(currentRole) &&
                           currentRole.Contains(SystemRoles.SuperAdmin);

        // Build role query considering SuperAdmin
        IQueryable<ApplicationRole> roleQuery = context.Roles;
        if (isSuperAdmin)
        {
            roleQuery = roleQuery.IgnoreQueryFilters();
        }

        var existingRole = roleQuery
            .Where(x => x.Id == roleId)
            .Select(y => new
            {
                y.Id,
                y.RoleType,
                y.Name
            })
            .FirstOrDefault();

        if (existingRole == null)
            return Result<RolePermissionViewModel>.Failed("Role not found.");

        var rolePermissionViewModel = new RolePermissionViewModel
        {
            RoleId = existingRole.Id,
            RoleName = existingRole.Name,
            RolePermissionGroup = new List<RolePermissionGroup>(),
        };

        // RoleClaims query with SuperAdmin consideration
        IQueryable<ApplicationRoleClaim> roleClaimQuery = context.RoleClaims;
        if (isSuperAdmin)
        {
            roleClaimQuery = roleClaimQuery.IgnoreQueryFilters();
        }

        var existingPermissions = roleClaimQuery
            .Where(q => q.RoleId == roleId)
            .Select(x => x.Permissions)
            .FirstOrDefault();

        // Filter menu permissions based on role type
        var groupedPermissions = MenuPermissionsList._list
            .Where(menu => menu.AllowedRoles == null ||
                           !menu.AllowedRoles.Any() ||
                           menu.AllowedRoles.Contains("All", StringComparer.OrdinalIgnoreCase) ||
                           menu.AllowedRoles.Any(allowedRole =>
                               string.Equals(allowedRole, existingRole.RoleType, StringComparison.OrdinalIgnoreCase) ||
                               (string.Equals(allowedRole, "SuperAdmin", StringComparison.OrdinalIgnoreCase) && existingRole.RoleType == "SuperAdmin") ||
                               (string.Equals(allowedRole, "Admin", StringComparison.OrdinalIgnoreCase) &&
                                (existingRole.RoleType == "Admin" || existingRole.RoleType == "TenantAdmin")) ||
                               (string.Equals(allowedRole, "FoDo", StringComparison.OrdinalIgnoreCase) &&
                                (existingRole.RoleType == "FoDo" || existingRole.RoleType == "MarketingExecutive"))
                           ))
            .ToList();

        foreach (var menuItem in groupedPermissions)
        {
            var rolePermissionGroup = rolePermissionViewModel.RolePermissionGroup
                .FirstOrDefault(group => group.Module == menuItem.MenuName);

            if (rolePermissionGroup == null)
            {
                rolePermissionGroup = new RolePermissionGroup
                {
                    Module = menuItem.MenuName,
                    Rank = menuItem.Rank,
                    HideChildren = menuItem.HideChildren
                };
                rolePermissionViewModel.RolePermissionGroup.Add(rolePermissionGroup);
            }

            AddPermissionsWithChildren(menuItem, existingPermissions, rolePermissionGroup, existingRole.RoleType);
        }

        return Result<RolePermissionViewModel>.Success(rolePermissionViewModel);
    }


    private void AddPermissionsWithChildren(MenuItem menuItem, List<string> permissions, RolePermissionGroup rolePermissionGroup, string roleType)
    {
        foreach (var permission in menuItem.Permissions)
        {
            PermissionList permissionList = new PermissionList
            {
                PermissionId = permission.Value,
                PermissionTitle = permission.Title,
                IsAutoCheck = permission.IsAutoCheck,
                HasClaim = (permissions != null) && permissions.Contains(permission.Value)
            };
            rolePermissionGroup.Permissions.Add(permissionList);
        }

        if (menuItem.Children != null)
        {
            foreach (var child in menuItem.Children)
            {
                RolePermissionGroup childRolePermissionGroup = new RolePermissionGroup
                {
                    Module = child.MenuName,
                    Rank = child.Rank,
                    HideChildren = child.HideChildren
                };
                rolePermissionGroup.Childrens.Add(childRolePermissionGroup);
                AddPermissionsWithChildren(child, permissions, childRolePermissionGroup, roleType);
            }
        }
    }

    public async Task<Result<MessageResponseModel>> AssignRolePermissionAsync(
    PermissionManagementViewModel requestModel,
    CancellationToken cancellationToken = default)
    {
        var roleName = personAccessor.GetRoleId();
        var isSuperAdmin = !string.IsNullOrWhiteSpace(roleName) &&
                           roleName.Contains(SystemRoles.SuperAdmin);

        var roleId = requestModel.RoleId;

        // Build role query based on SuperAdmin
        IQueryable<ApplicationRole> roleQuery = context.Roles;
        if (isSuperAdmin)
        {
            roleQuery = roleQuery.IgnoreQueryFilters();
        }

        var hasRole = await roleQuery
            .AnyAsync(x => x.Id == roleId, cancellationToken);

        if (!hasRole)
        {
            return Result<MessageResponseModel>.Failed("Role not found.");
        }

        // Build role-claim query based on SuperAdmin
        IQueryable<ApplicationRoleClaim> roleClaimQuery = context.RoleClaims;
        if (isSuperAdmin)
        {
            roleClaimQuery = roleClaimQuery.IgnoreQueryFilters();
        }

        var roleClaim = await roleClaimQuery
            .FirstOrDefaultAsync(x => x.RoleId == roleId, cancellationToken);

        if (roleClaim is null)
        {
            context.RoleClaims.Add(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = requestModel.ClaimList
            });
        }
        else
        {
            roleClaim.Permissions = requestModel.ClaimList;
            // No need to call Update — EF tracks changes automatically
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result<MessageResponseModel>.Success(
            new MessageResponseModel("Permissions of role added successfully!")
        );
    }
}
