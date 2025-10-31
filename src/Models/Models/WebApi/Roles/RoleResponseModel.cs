namespace Models.BeemaEdgeApi.Roles;
public class RoleResponseModel
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public string RoleDescription { get; set; }
    public string RoleType { get; set; }
    public int TotalUserAssignedWithRole { get; set; }
}
