using System.ComponentModel.DataAnnotations;

namespace Models.BeemaEdgeApi.Identity;

public class ChangePasswordRequestModel
{
    [Required(ErrorMessage = "Old password is required."), DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required."), DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required."), DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}

public class SetPasswordRequestModel
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Token { get; set; }

    [Required(ErrorMessage = "New password is required."), DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required."), DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}

