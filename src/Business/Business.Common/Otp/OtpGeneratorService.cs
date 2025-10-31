using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Business.Common.StringCipher;
using Data.Context;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SharedKernel.SystemEnum.Otp;

namespace Business.Common.Otp;
public class OtpGeneratorService
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ApplicationDataContext _dbContext;
    private readonly StringCipherService _stringCipherService;
    private readonly int otpExpiryTimeInSeconds;


    public OtpGeneratorService(IHostEnvironment hostEnvironment,
        StringCipherService stringCipherService,
        ApplicationDataContext dbContext,
        IOptions<LoginSettings> loginSettings)
    {
        _hostEnvironment = hostEnvironment;
        _stringCipherService = stringCipherService;
        _dbContext = dbContext;
        otpExpiryTimeInSeconds = loginSettings.Value.OtpExpiryTimeInSeconds;

    }

    /// <summary>
    /// Generates OTP with its encrypted value in production. For other environment its 9999,9999
    /// </summary>
    /// <param name="length"></param>
    /// <returns>Tuple with OTP code (item1) & encryptedcode (item2) </returns>
    public async Task<Tuple<string, string>> GenerateOtpAsync(string phoneNumber = null, int length = 6)
    {

        //if (!_hostEnvironment.IsProduction() || _hostEnvironment.EnvironmentName.ToLower() == "dev")
        //    return new("9999", "9999");

        var otpBuilder = new StringBuilder();
        var randomNumberBuffer = new byte[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumberBuffer);

            for (int i = 0; i < length; i++)
            {
                int digit = randomNumberBuffer[i] % 10;
                otpBuilder.Append(digit);
            }
        }

        var otpCode = otpBuilder.ToString();
        var otpCodeEncrypted = await _stringCipherService.EncryptAsync(otpCode);

        return new(otpCode, otpCodeEncrypted);
    }

    public async Task<bool> ValidateOtpAsync(string storedOtp, string otp, DateTime otpSentTime)
    {
        if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(otp))
            return false;

        var otpCodeDecrypted = storedOtp;

        Console.WriteLine(_hostEnvironment.EnvironmentName.ToLower());
        //comment below for live-loadtest
        //if (_hostEnvironment.IsProduction() || _hostEnvironment.EnvironmentName.ToLower() == "prod")
        otpCodeDecrypted = await _stringCipherService.DecryptAsync(storedOtp);

        bool isExpired = DateTimeOffset.UtcNow > otpSentTime.AddSeconds(otpExpiryTimeInSeconds);

        if (isExpired)
            return false;

        if (otp == otpCodeDecrypted)
            return true;

        return false;
    }
    public bool ValidateOtpTime(DateTime otpSentTime)
    {

        bool isExpired = DateTimeOffset.UtcNow > otpSentTime.AddSeconds(otpExpiryTimeInSeconds);

        return !isExpired;
    }

    public async Task<string> AddOtpAsync(string userId, string otpCode, OtpType otpType, OtpChannel channel, SystemModule module, DateTime otpSentDateTime)
    {
        var userOtpDetail = await _dbContext.UserOtps.FirstOrDefaultAsync(x => x.Type == otpType && x.UserId == userId);

        if (userOtpDetail == null)
        {
            var otp = new UserOtp(userId, otpCode, otpType, channel, module, otpSentDateTime);
            await _dbContext.UserOtps.AddAsync(otp);
            await _dbContext.SaveChangesAsync();

            return otp.OTPToken;
        }
        else
        {
            userOtpDetail.OTPCode = otpCode;
            userOtpDetail.OTPSentDateTime = otpSentDateTime;
            userOtpDetail.IsUsed = false;
            userOtpDetail.OTPToken = Guid.NewGuid().ToString();

            _dbContext.UserOtps.Update(userOtpDetail);
            await _dbContext.SaveChangesAsync();

            return userOtpDetail.OTPToken;
        }
    }

    public async Task<bool> IsOtpValidAndExpired(string storedOtp, string otp, DateTime otpSentTime)
    {
        if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(otp))
            return false;

        var otpCodeDecrypted = storedOtp;

        //comment below for live-loadtest
        //if (_hostEnvironment.IsProduction() || _hostEnvironment.EnvironmentName.ToLower() != "dev")
        otpCodeDecrypted = await _stringCipherService.DecryptAsync(storedOtp);

        bool isExpired = DateTimeOffset.UtcNow > otpSentTime.AddSeconds(otpExpiryTimeInSeconds);

        if (isExpired && otp == otpCodeDecrypted)
            return true;

        return false;
    }
}

public class LoginSettings
{
    public int OtpExpiryTimeInSeconds { get; set; }
}
public class VerifyOtpRequestModel
{
    [Required(ErrorMessage = "Otp type is required")]
    public OtpType OtpType { get; set; }
    [Required(ErrorMessage = "Otp  is required")]
    public string Otp { get; set; }
    [Required(ErrorMessage = "Username type is required")]
    public string Username { get; set; }
}
public class VerifyCustomerOtpRequestModel
{
    [Required(ErrorMessage = "Otp is required")]
    public string Otp { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }
}
public class ResendOtpRequestModel
{
    [Required(ErrorMessage = "Otp is required")]
    public OtpType OtpType { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }

}

