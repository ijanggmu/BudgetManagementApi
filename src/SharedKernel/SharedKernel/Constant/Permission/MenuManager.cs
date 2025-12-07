using System.Collections.Immutable;

namespace SharedKernel.Constant.Permission;

public static partial class MenuPermissionsList
{
    public static class MenuManager
    {
        public static List<MenuItem> GetMenusForPermissions(IEnumerable<string> permissionIds)
        {
            List<MenuItem> filteredMenus = MenuPermissionsList._list
                .Where(menu => menu.Permissions.Any(x => permissionIds.Contains(x.Value)))
                .Select(menu => new MenuItem
                {
                    MenuId = menu.MenuId,
                    MenuName = menu.MenuName,
                    MenuSlug = menu.MenuSlug,
                    Icon = menu.Icon,
                    Rank = menu.Rank,
                    Permissions = menu.Permissions.Where(x => permissionIds.Contains(x.Value)).ToList(),
                    Children = menu.Children != null ? GetMenusForPermissionsFromChildren(menu.Children, permissionIds) : null,
                    IsDisabled = menu.IsDisabled,
                    IsMenu = menu.IsMenu,
                    Level = menu.Level,
                    ToHide = menu.ToHide,
                    HideChildren = menu.HideChildren,
                }).ToList();

            return filteredMenus;
        }

        private static List<MenuItem> GetMenusForPermissionsFromChildren(IEnumerable<MenuItem> children, IEnumerable<string> permissionIds)
        {
            List<MenuItem> filteredChildren = children
                .Where(child => child.Permissions.Any(x => permissionIds.Contains(x.Value)))
                .Select(child => new MenuItem
                {
                    MenuId = child.MenuId,
                    MenuName = child.MenuName,
                    MenuSlug = child.MenuSlug,
                    Icon = child.Icon,
                    Rank = child.Rank,
                    Permissions = child.Permissions.Where(x => permissionIds.Contains(x.Value)).ToList(),
                    Children = child.Children != null ? GetMenusForPermissionsFromChildren(child.Children, permissionIds) : null,
                    IsDisabled = child.IsDisabled,
                    IsMenu = child.IsMenu,
                    Level = child.Level,
                    ToHide = child.ToHide,
                    HideChildren = child.HideChildren
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

