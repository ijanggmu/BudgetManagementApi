using System.ComponentModel.DataAnnotations;
using Business.Common.CustomAttribute;
using SharedKernel.Validation;

namespace Models.BeemaEdgeApi.Identity;

public class RegisterRequestModel
{
    [Required]
    [StringLength(32)]
    public string UserName { get; set; }

    [Required]
    [StringLength(32)]
    public string FullName { get; set; }

    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    public string MobileNumber { get; set; }

}


public class RegisterCustomerRequestModel
{

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(64, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 64 characters.")]
    public string FullName { get; set; }

    //[Required(ErrorMessage = "Email is required.")]
    //[EmailAddress(ErrorMessage = "Invalid email address format.")]
    [OptionalEmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Mobile number is required.")]
    [Phone(ErrorMessage = "Invalid mobile number format.")]
    public string MobileNumber { get; set; }

    [Required(ErrorMessage = "Country id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Country id must be greater than 0.")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

}
