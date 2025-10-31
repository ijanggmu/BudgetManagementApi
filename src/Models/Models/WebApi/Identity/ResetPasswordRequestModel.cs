using System.ComponentModel.DataAnnotations;

namespace Models.BeemaEdgeApi.Identity;

public class ResetPasswordRequestModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    public string ConfirmPassword { get; set; }
}
