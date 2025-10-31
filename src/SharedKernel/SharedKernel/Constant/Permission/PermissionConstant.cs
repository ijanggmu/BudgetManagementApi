namespace SharedKernel.Constant.Permission;
public static class MenuPermissionConstant
{
    //format ParentMenuId-OperationType
    //format ParentMenuId-ChildMenuId-OperationType
    //format ParentMenuId-ChildMenuId-GrandChildMenuId-OperationType

    public enum MenuCount
    {
        Todo
    }

    //OperationType count = 44 //take next value
    public enum OperationType
    {
        View = 1,
        Create,
        Update,
        Delete,
    }


    #region Dashboard
    public const string TodoViewName = "Todo";
    public const string TodoView = "1-1";

    public const string TodoCreateName = "Todo Create";
    public const string TodoCreate = "1-2";

    public const string TodoUpdateName = "Todo Update";
    public const string TodoUpdate = "1-3";

    public const string TodoDeleteName = "Roles Delete";
    public const string TodoDelete = "1-4";
    #endregion

}
public static class CmsMenuConstant
{
    public const string Todo = "Todo";
}
public enum MenuRank
{
    Todo = 1,
}
public class MenuItem
{
    public int MenuId { get; set; }
    public string MenuName { get; set; }
    public string MenuSlug { get; set; }
    public string Icon { get; set; }
    public int Rank { get; set; }
    public int Level { get; set; }

    public bool IsDisabled { get; set; } = false;
    public bool IsMenu { get; set; } = true;
    public bool ToHide { get; set; } = false;
    public bool HideChildren { get; set; }


    public List<Permission> Permissions { get; set; }
    public List<MenuItem> Children { get; set; }
}
public class Permission
{
    public string Title { get; set; }
    public string Value { get; set; }
    public bool IsAutoCheck { get; set; }

    public Permission(string title, string value, bool isAutoCheck = false)
    {
        Title = title;
        Value = value;
        IsAutoCheck = isAutoCheck;
    }
}
