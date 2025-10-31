using System.ComponentModel.DataAnnotations;

namespace Models.BeemaEdgeApi.Customer.CustomerIdentity;

public class ForgetPasswordRequestModel
{
    [Required]
    public string UserName { get; set; }
}
