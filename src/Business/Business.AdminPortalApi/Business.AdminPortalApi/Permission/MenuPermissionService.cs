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

        // The caller's own privilege caps what they can see/assign, regardless of which
        // target role they're editing — a tenant admin must never see SuperAdmin-only
        // permissions (e.g. Tenants management), even when editing the SuperAdmin role.
        var isCallerSuperAdmin = _personAccessor.IsSuperAdmin();

        // Filter menus based on the role's AllowedRoles and the caller's own privilege
        var groupedPermissions = MenuPermissionsList._list
            .Where(menu => IsMenuAllowedForRole(menu, roleForFiltering) && IsVisibleToCaller(menu, isCallerSuperAdmin))
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

            AddPermissionsWithChildren(menuItem, existingPermissions, rolePermissionGroup, roleForFiltering, isCallerSuperAdmin);
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

    /// <summary>
    /// A non-SuperAdmin caller must never see/assign a menu item that is exclusively
    /// restricted to SuperAdmin (e.g. Tenants management), no matter which role they're editing.
    /// </summary>
    private static bool IsVisibleToCaller(MenuItem menu, bool isCallerSuperAdmin)
    {
        if (isCallerSuperAdmin)
            return true;

        if (menu.AllowedRoles == null || !menu.AllowedRoles.Any())
            return true;

        if (menu.AllowedRoles.Contains("All", StringComparer.OrdinalIgnoreCase))
            return true;

        var isSuperAdminOnly = menu.AllowedRoles.All(r => string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));
        return !isSuperAdminOnly;
    }

    /// <summary>Permission ids belonging to menu items restricted exclusively to SuperAdmin.</summary>
    private static HashSet<string> GetSuperAdminOnlyPermissionIds()
    {
        var ids = new HashSet<string>();

        void Collect(IEnumerable<MenuItem> items)
        {
            foreach (var item in items)
            {
                if (!IsVisibleToCaller(item, isCallerSuperAdmin: false))
                {
                    foreach (var permission in item.Permissions)
                        ids.Add(permission.Value);
                }

                if (item.Children != null)
                    Collect(item.Children);
            }
        }

        Collect(MenuPermissionsList._list);
        return ids;
    }

    private void AddPermissionsWithChildren(MenuItem menuItem, List<string> permissions, RolePermissionGroup rolePermissionGroup, string? roleForFiltering = null, bool isCallerSuperAdmin = true)
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
            // Filter children based on role's AllowedRoles and the caller's own privilege
            var filteredChildren = menuItem.Children
                .Where(child => IsMenuAllowedForRole(child, roleForFiltering) && IsVisibleToCaller(child, isCallerSuperAdmin))
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
                AddPermissionsWithChildren(child, permissions, childRolePermissionGroup, roleForFiltering, isCallerSuperAdmin);
            }
        }
    }

    public async Task<Result<MessageResponseModel>> AssignRolePermissionAsync(PermissionManagementViewModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = requestModel.RoleId;

        var hasRole = await _context.Roles.AnyAsync(x => x.Id == roleId, cancellationToken);
        if (!hasRole)
            return Result<MessageResponseModel>.Failed("Role not found.");

        var claimList = requestModel.ClaimList ?? new List<string>();

        // Defense in depth: even if a request bypasses the UI, a non-SuperAdmin caller
        // can never grant a SuperAdmin-only permission to any role.
        if (!_personAccessor.IsSuperAdmin())
        {
            var superAdminOnlyIds = GetSuperAdminOnlyPermissionIds();
            claimList = claimList.Where(c => !superAdminOnlyIds.Contains(c)).ToList();
        }

        var roleClaim = await _context.RoleClaims.Where(x => x.RoleId == roleId).FirstOrDefaultAsync(cancellationToken);

        if (roleClaim == null)
        {
            _context.RoleClaims.Add(new ApplicationRoleClaim
            {
                RoleId = roleId,
                Permissions = claimList
            });
        }
        else
        {
            roleClaim.Permissions = claimList;
            _context.RoleClaims.Update(roleClaim);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Permissions of role added successfully!"));
    }
}
