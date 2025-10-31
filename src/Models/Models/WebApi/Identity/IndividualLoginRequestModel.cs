using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.BeemaEdgeApi.Identity;
public class IndividualLoginRequestModel
{
    [Required(ErrorMessage = "Phonenumber is required.")]
    [DefaultValue("9999999999")]
    [Phone(ErrorMessage = "Invalid Phone number format.")]
    public string Username { get; set; }

    [DefaultValue("Admin@123")]
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}
public class AdminLoginRequestModel
{
    [DefaultValue("superadmin")]
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }

    [DefaultValue("Admin@123")]
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}
public class AgentLoginRequestModel
{
    [DefaultValue("superagent")]
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }

    [DefaultValue("Admin@123")]
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}
public class CorporateLoginRequestModel
{
    [DefaultValue("supercorporate")]
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }

    [DefaultValue("Admin@123")]
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}
public class Verify2FaCustomerRequestModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Code { get; set; }
}
public class Verify2FaAdminRequestModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Code { get; set; }
}
public class LoginCustomerResponseModel
{
    public string Token { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
}
