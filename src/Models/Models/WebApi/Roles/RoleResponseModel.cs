namespace Models.BeemaEdgeApi.Roles;

public class RoleResponseModel
{
    public DateTime CreatedOn { get; set; }
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public string RoleDisplayName { get; set; }
    public string RoleDescription { get; set; }
    public string RoleType { get; set; }
    public int TotalUserAssignedWithRole { get; set; }
    public string TenantId { get; set; }
    public string TenantName { get; set; }
}
