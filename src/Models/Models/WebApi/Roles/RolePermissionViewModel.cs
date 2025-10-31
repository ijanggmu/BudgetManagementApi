namespace Models.BeemaEdgeApi.Roles;
public class RolePermissionViewModel
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public List<RolePermissionGroup> RolePermissionGroup { get; set; } = new List<RolePermissionGroup>();
}

public class RolePermissionGroup
{
    public string Module { get; set; }
    public int Rank { get; set; }
    public bool HideChildren { get; set; }

    public List<PermissionList> Permissions { get; set; } = new List<PermissionList>();
    public List<RolePermissionGroup> Childrens { get; set; } = new List<RolePermissionGroup>();
}

public class PermissionList
{
    public string PermissionId { get; set; }
    public string PermissionTitle { get; set; }
    public bool HasClaim { get; set; }
    public bool IsAutoCheck { get; set; }
}

public class PermissionManagementViewModel
{
    public string RoleId { get; set; }
    public List<string> ClaimList { get; set; }
}
