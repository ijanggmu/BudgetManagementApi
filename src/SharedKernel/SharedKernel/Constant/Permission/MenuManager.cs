using System;
using System.Collections.Immutable;
using System.Linq;

namespace SharedKernel.Constant.Permission;

public static partial class MenuPermissionsList
{
    public static class MenuManager
    {
        public static List<MenuItem> GetMenusForPermissions(IEnumerable<string> permissionIds, List<string>? userRoles = null)
        {
            List<MenuItem> filteredMenus = MenuPermissionsList._list
                .Where(menu => menu.Permissions.Any(x => permissionIds.Contains(x.Value))
                    && IsMenuAllowedForRoles(menu, userRoles))
                .Select(menu => new MenuItem
                {
                    MenuId = menu.MenuId,
                    MenuName = menu.MenuName,
                    MenuSlug = menu.MenuSlug,
                    Icon = menu.Icon,
                    Rank = menu.Rank,
                    Permissions = menu.Permissions.Where(x => permissionIds.Contains(x.Value)).ToList(),
                    Children = menu.Children != null ? GetMenusForPermissionsFromChildren(menu.Children, permissionIds, userRoles) : null,
                    IsDisabled = menu.IsDisabled,
                    IsMenu = menu.IsMenu,
                    Level = menu.Level,
                    ToHide = menu.ToHide,
                    HideChildren = menu.HideChildren,
                    AllowedRoles = menu.AllowedRoles,
                }).ToList();

            return filteredMenus;
        }

        /// <summary>
        /// Checks if a menu item is allowed for the given user roles.
        /// If AllowedRoles is null or empty, menu is visible to all roles.
        /// If AllowedRoles contains "All", menu is visible to all roles.
        /// Otherwise, menu is visible only if user has one of the allowed roles.
        /// </summary>
        private static bool IsMenuAllowedForRoles(MenuItem menu, List<string>? userRoles)
        {
            // If no role restriction, allow for all
            if (menu.AllowedRoles == null || !menu.AllowedRoles.Any())
                return true;

            // If "All" is in allowed roles, allow for all
            if (menu.AllowedRoles.Contains("All", StringComparer.OrdinalIgnoreCase))
                return true;

            // If no user roles provided, deny access
            if (userRoles == null || !userRoles.Any())
                return false;

            // Check if user has any of the allowed roles
            return menu.AllowedRoles.Any(allowedRole => 
                userRoles.Any(userRole => 
                    string.Equals(allowedRole, userRole, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(allowedRole, "SuperAdmin", StringComparison.OrdinalIgnoreCase) && userRole == "SuperAdmin" ||
                    string.Equals(allowedRole, "Admin", StringComparison.OrdinalIgnoreCase) && (userRole == "Admin" || userRole == "TenantAdmin") ||
                    string.Equals(allowedRole, "FoDo", StringComparison.OrdinalIgnoreCase) && (userRole == "FoDo" || userRole == "MarketingExecutive")
                ));
        }

        private static List<MenuItem> GetMenusForPermissionsFromChildren(IEnumerable<MenuItem> children, IEnumerable<string> permissionIds, List<string>? userRoles = null)
        {
            List<MenuItem> filteredChildren = children
                .Where(child => child.Permissions.Any(x => permissionIds.Contains(x.Value))
                    && IsMenuAllowedForRoles(child, userRoles))
                .Select(child => new MenuItem
                {
                    MenuId = child.MenuId,
                    MenuName = child.MenuName,
                    MenuSlug = child.MenuSlug,
                    Icon = child.Icon,
                    Rank = child.Rank,
                    Permissions = child.Permissions.Where(x => permissionIds.Contains(x.Value)).ToList(),
                    Children = child.Children != null ? GetMenusForPermissionsFromChildren(child.Children, permissionIds, userRoles) : null,
                    IsDisabled = child.IsDisabled,
                    IsMenu = child.IsMenu,
                    Level = child.Level,
                    ToHide = child.ToHide,
                    HideChildren = child.HideChildren,
                    AllowedRoles = child.AllowedRoles,
                }).ToList();

            return filteredChildren;
        }


        public static List<Permission> GetAllPermissions()
        {
            List<Permission> permissions = new List<Permission>();
            TraverseMenu(MenuPermissionsList._list, permissions);
            return permissions;
        }

        private static void TraverseMenu(IEnumerable<MenuItem> menuItems, List<Permission> permissions)
        {
            foreach (var menuItem in menuItems)
            {
                permissions.AddRange(menuItem.Permissions);
                if (menuItem.Children != null && menuItem.Children.Count > 0)
                {
                    TraverseMenu(menuItem.Children, permissions);
                }
            }
        }
    }
}

