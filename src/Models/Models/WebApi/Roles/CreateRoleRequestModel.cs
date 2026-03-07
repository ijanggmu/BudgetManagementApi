using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Models.BeemaEdgeApi.Roles;

public class CreateRoleRequestModel
{
    [DefaultValue("Role")]
    [Required(ErrorMessage = "Role Name is required")]
    [RegularExpression(@"^\s*(\w+\s)*\w+\s*$", ErrorMessage = "Can contain only one space")]
    public string RoleName { get; set; }
    /// <summary>Optional display name (e.g. "Chief Executive Officer" for CEO).</summary>
    public string RoleDisplayName { get; set; }
    public string RoleDescription { get; set; }
    public string RoleType { get; set; }
    //TODO::
    // public string RoleLevel { get; set; }
}
