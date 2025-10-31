using Data.Context;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OtpNet;

namespace Business.Common.Totp;
public class TotpConfig
{
    public int Digits { get; set; } = 6;
    public int Step { get; set; } = 30;
    public string Issuer { get; set; } = "CustomerPortal";

}
public class TotpService(IOptions<TotpConfig> totpConfig,
    UserManager<ApplicationUser> userManager,
    IHostEnvironment hostingEnvironment, ApplicationDataContext context) : ITotpService
{
    private readonly TotpConfig _totpConfig = totpConfig.Value;

    public async Task<string> GenerateTotpQrCode(ApplicationUser user)
    {
        user.TotpSecurityStamp = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
        user.TwoFaSetupStatus = TwoFaSetupStatus.SetupGenerated;
        await userManager.UpdateAsync(user);

        return $"otpauth://totp/{_totpConfig.Issuer}:{user.Email}?secret={user.TotpSecurityStamp}&issuer={_totpConfig.Issuer}&digits={_totpConfig.Digits}&period={_totpConfig.Step}";
    }

    public bool ValidateTotpCode(string token, string code)
    {
        //if (!hostingEnvironment.IsProduction() && hostingEnvironment.EnvironmentName.ToLower() != "Prod" && code == "999999")
        //    return true;

        var totp = new OtpNet.Totp(Base32Encoding.ToBytes(token), step: _totpConfig.Step, totpSize: _totpConfig.Digits);

        return totp.VerifyTotp(code, out _);
    }

    public async Task Disable2FaAsync(ApplicationUser user)
    {
        user.UserTotpBackUpCodes = null;
        user.TwoFactorEnabled = false;
        user.TwoFaSetupStatus = TwoFaSetupStatus.NotStarted;
        await userManager.SetTwoFactorEnabledAsync(user, false);
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

}

