using Data.Entities.Identity;

namespace Business.Common.Totp;
public interface ITotpService
{
    Task<string> GenerateTotpQrCode(ApplicationUser user);
    bool ValidateTotpCode(string token, string code);
    Task Disable2FaAsync(ApplicationUser user);

}

