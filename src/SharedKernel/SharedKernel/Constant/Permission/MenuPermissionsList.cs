using System.Collections.Immutable;

namespace SharedKernel.Constant.Permission;

public static partial class MenuPermissionsList
{
    public static readonly ImmutableList<MenuItem> _list = ImmutableList<MenuItem>.Empty
        .AddRange(new List<MenuItem>
        {
                new MenuItem
                {
                    MenuId = 1,
                    MenuName = MenuPermissionConstant.TodoView,
                    MenuSlug = "/",
                    Icon = "HomeOutlined",
                    Rank = (int)MenuRank.Todo,
                    Level = 1,
                    Permissions = new List<Permission> {
                        new Permission(MenuPermissionConstant.TodoViewName, MenuPermissionConstant.TodoView, true),
                        new Permission(MenuPermissionConstant.TodoCreateName, MenuPermissionConstant.TodoCreate),
                        new Permission(MenuPermissionConstant.TodoUpdateName, MenuPermissionConstant.TodoUpdate),
                        new Permission(MenuPermissionConstant.TodoDeleteName, MenuPermissionConstant.TodoDelete),
                    }
                } });

}
