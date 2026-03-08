using System.Collections.Immutable;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Roles;
using Models.Common;
using Models.Common.Menu;
using SharedKernel.Constant.Permission;
using SharedKernel.Operation;
using static SharedKernel.Constant.Permission.MenuPermissionsList;

namespace Business.AdminPortalApi.Permission;

public class MenuPermissionService : IMenuPermissionService
{
    private readonly ApplicationDataContext _context;
    private readonly IUserProfileService _personAccessor;

    public MenuPermissionService(ApplicationDataContext context,
        IUserProfileService personAccessor)
    {
        _context = context;
        _personAccessor = personAccessor;
    }

    public Result<MenuModel> GetMenu()
    {
        var roleId = _personAccessor.GetRoleId();

        if (string.IsNullOrEmpty(roleId))
            return Result<MenuModel>.Success(new MenuModel());

        var roleNameList = roleId.Split(",").Select(r => r.Trim()).ToList();

        var permissionList = (from role in _context.Roles
                              join roleClaim in _context.RoleClaims
                              on role.Id equals roleClaim.RoleId
                              where roleNameList.Contains(role.Name) && !role.IsDeleted
                              select roleClaim.Permissions)
                          .ToList();

        var permissions = permissionList.SelectMany(x => x).ToList();

        if (!permissions.Any())
            return Result<MenuModel>.Success(new MenuModel());

        // Get user roles for menu filtering
        var userRoles = roleNameList.ToList();

        var menus = MenuManager.GetMenusForPermissions(permissions, userRoles);

        return Result<MenuModel>.Success(new MenuModel { MenuList = menus });

    }

    public Result<RolePermissionViewModel> GetAllMenuByRoleId(string roleId)
    {
        var existingRole = _context.Roles.Where(x => x.Id == roleId).Select(y => new
        {
            y.Id,
            y.RoleType,
            y.Name
        }).FirstOrDefault();

        if (existingRole == null)
            return Result<RolePermissionViewModel>.Failed("Role not found.");

        RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel
        {
            RoleId = existingRole.Id,
            RoleName = existingRole.Name,
            RolePermissionGroup = new List<RolePermissionGroup>()
        };

        var existingPermissions = _context.RoleClaims
                                                 .Where(q => q.RoleId == roleId)
                                                 .Select(x => x.Permissions)
                                                 .FirstOrDefault();

        // Get the role name and map it to the AllowedRoles format
        var roleName = existingRole.Name;
        var roleForFiltering = MapRoleNameToAllowedRole(roleName);

        // Filter menus based on the role's AllowedRoles
        var groupedPermissions = MenuPermissionsList._list
            .Where(menu => IsMenuAllowedForRole(menu, roleForFiltering))
            .ToList();

        foreach (var menuItem in groupedPermissions)
        {
            RolePermissionGroup rolePermissionGroup = rolePermissionViewModel.RolePermissionGroup
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

            AddPermissionsWithChildren(menuItem, existingPermissions, rolePermissionGroup, roleForFiltering);
        }

        return Result<RolePermissionViewModel>.Success(rolePermissionViewModel);


    }

    /// <summary>
    /// Maps role name to the AllowedRoles format used in menu configuration
    /// </summary>
    private string? MapRoleNameToAllowedRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return null;

        // Map role names to AllowedRoles format
        return roleName switch
        {
            var name when name.Equals(SharedKernel.Constant.Roles.SystemRoles.SuperAdmin, StringComparison.OrdinalIgnoreCase) => "SuperAdmin",
            var name when name.Equals(SharedKernel.Constant.Roles.SystemRoles.Admin, StringComparison.OrdinalIgnoreCase) => "Admin",
            _ => null
        };
    }

    /// <summary>
    /// Checks if a menu item is allowed for the given role
    /// </summary>
    private bool IsMenuAllowedForRole(MenuItem menu, string? roleForFiltering)
    {
        // If no role restriction, allow for all
        if (menu.AllowedRoles == null || !menu.AllowedRoles.Any())
            return true;

        // If "All" is in allowed roles, allow for all
        if (menu.AllowedRoles.Contains("All", StringComparer.OrdinalIgnoreCase))
            return true;

        // If no role provided, deny access
        if (string.IsNullOrWhiteSpace(roleForFiltering))
            return false;

        // Check if role matches any of the allowed roles
        return menu.AllowedRoles.Any(allowedRole =>
            string.Equals(allowedRole, roleForFiltering, StringComparison.OrdinalIgnoreCase)
        );
    }

    private void AddPermissionsWithChildren(MenuItem menuItem, List<string> permissions, RolePermissionGroup rolePermissionGroup, string? roleForFiltering = null)
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
            // Filter children based on role's AllowedRoles
            var filteredChildren = menuItem.Children
                .Where(child => IsMenuAllowedForRole(child, roleForFiltering))
                .ToList();

            foreach (var child in filteredChildren)
            {
                RolePermissionGroup childRolePermissionGroup = new RolePermissionGroup
                {
                    Module = child.MenuName,
                    Rank = child.Rank,
                    HideChildren = child.HideChildren
                };
                rolePermissionGroup.Childrens.Add(childRolePermissionGroup);
                AddPermissionsWithChildren(child, permissions, childRolePermissionGroup, roleForFiltering);
            }
        }
    }

    public async Task<Result<MessageResponseModel>> AssignRolePermissionAsync(PermissionManagementViewModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = requestModel.RoleId;

        var hasRole = await _context.Roles.AnyAsync(x => x.Id == roleId, cancellationToken);
        if (!hasRole)
            return Result<MessageResponseModel>.Failed("Role not found.");

        var roleClaim = await _context.RoleClaims.Where(x => x.RoleId == roleId).FirstOrDefaultAsync(cancellationToken);

        if (roleClaim == null)
        {
            _context.RoleClaims.Add(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = requestModel.ClaimList
            });
        }
        else
        {
            roleClaim.Permissions = requestModel.ClaimList;
            _context.RoleClaims.Update(roleClaim);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Permissions of role added successfully!"));
    }
}
