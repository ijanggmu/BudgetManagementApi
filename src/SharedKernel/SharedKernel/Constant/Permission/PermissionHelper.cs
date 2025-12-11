namespace SharedKernel.Constant.Permission;

/// <summary>
/// Helper class for generating and managing permissions in a maintainable way
/// </summary>
public static class PermissionHelper
{
    /// <summary>
    /// Generates all CRUD + Export permissions for a menu item
    /// </summary>
    /// <param name="menuPath">Menu path in format "Parent-Child" or "Parent-Child-GrandChild"</param>
    /// <param name="menuName">Display name of the menu</param>
    /// <returns>Permission set with View, Create, Update, Delete, and Export permissions</returns>
    public static MenuPermissionSet GeneratePermissions(string menuPath, string menuName)
    {
        return new MenuPermissionSet
        {
            View = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.View}",
            Create = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Create}",
            Update = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Update}",
            Delete = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Delete}",
            Export = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Export}",
            ViewName = $"{menuName} View",
            CreateName = $"{menuName} Create",
            UpdateName = $"{menuName} Update",
            DeleteName = $"{menuName} Delete",
            ExportName = $"{menuName} Export"
        };
    }

    /// <summary>
    /// Generates permissions for a menu item with only specific operations
    /// </summary>
    public static MenuPermissionSet GeneratePermissions(
        string menuPath, 
        string menuName, 
        bool includeView = true,
        bool includeCreate = true,
        bool includeUpdate = true,
        bool includeDelete = true,
        bool includeExport = true)
    {
        var set = new MenuPermissionSet();
        
        if (includeView)
        {
            set.View = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.View}";
            set.ViewName = $"{menuName} View";
        }
        
        if (includeCreate)
        {
            set.Create = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Create}";
            set.CreateName = $"{menuName} Create";
        }
        
        if (includeUpdate)
        {
            set.Update = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Update}";
            set.UpdateName = $"{menuName} Update";
        }
        
        if (includeDelete)
        {
            set.Delete = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Delete}";
            set.DeleteName = $"{menuName} Delete";
        }
        
        if (includeExport)
        {
            set.Export = $"{menuPath}-{(int)MenuPermissionConstant.OperationType.Export}";
            set.ExportName = $"{menuName} Export";
        }
        
        return set;
    }

    /// <summary>
    /// Creates a list of Permission objects from a MenuPermissionSet
    /// </summary>
    public static List<Permission> ToPermissionList(MenuPermissionSet permissionSet, bool isAutoCheck = false)
    {
        var permissions = new List<Permission>();
        
        if (!string.IsNullOrEmpty(permissionSet.View))
            permissions.Add(new Permission(permissionSet.ViewName, permissionSet.View, isAutoCheck));
        
        if (!string.IsNullOrEmpty(permissionSet.Create))
            permissions.Add(new Permission(permissionSet.CreateName, permissionSet.Create, isAutoCheck));
        
        if (!string.IsNullOrEmpty(permissionSet.Update))
            permissions.Add(new Permission(permissionSet.UpdateName, permissionSet.Update, isAutoCheck));
        
        if (!string.IsNullOrEmpty(permissionSet.Delete))
            permissions.Add(new Permission(permissionSet.DeleteName, permissionSet.Delete, isAutoCheck));
        
        if (!string.IsNullOrEmpty(permissionSet.Export))
            permissions.Add(new Permission(permissionSet.ExportName, permissionSet.Export, isAutoCheck));
        
        return permissions;
    }
}

/// <summary>
/// Represents a complete set of permissions for a menu item
/// </summary>
public class MenuPermissionSet
{
    public string View { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
    public string Export { get; set; }
    
    public string ViewName { get; set; }
    public string CreateName { get; set; }
    public string UpdateName { get; set; }
    public string DeleteName { get; set; }
    public string ExportName { get; set; }
    
    /// <summary>
    /// Gets all permission values as a list
    /// </summary>
    public List<string> GetAllValues()
    {
        var values = new List<string>();
        if (!string.IsNullOrEmpty(View)) values.Add(View);
        if (!string.IsNullOrEmpty(Create)) values.Add(Create);
        if (!string.IsNullOrEmpty(Update)) values.Add(Update);
        if (!string.IsNullOrEmpty(Delete)) values.Add(Delete);
        if (!string.IsNullOrEmpty(Export)) values.Add(Export);
        return values;
    }
}

