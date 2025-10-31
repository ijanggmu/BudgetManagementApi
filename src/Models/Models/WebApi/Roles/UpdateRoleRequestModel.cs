using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Models.BeemaEdgeApi.Roles;

public class UpdateRoleRequestModel
{
    [Required]
    public string RoleId { get; set; }

    [DefaultValue("Role")]
    [Required(ErrorMessage = "Role Name is required")]
    [RegularExpression(@"^\s*(\w+\s)*\w+\s*$", ErrorMessage = "Can contain only one space")]
    public string RoleName { get; set; }

    public string RoleDescription { get; set; }

}
