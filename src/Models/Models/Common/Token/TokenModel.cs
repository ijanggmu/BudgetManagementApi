using System.ComponentModel.DataAnnotations;

namespace Models.Common.Token;
public class TokenModel
{
    [Required]
    public string AccessToken { get; set; }
    public int AccessTokenExpiryInSeconds { get; set; }

    [Required]
    public string RefreshToken { get; set; }
    public int RefreshTokenExpiryInSeconds { get; set; }
}
