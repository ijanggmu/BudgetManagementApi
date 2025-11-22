using Business.Common.Otp;
using Business.Common.Sms;
using Business.Common.Token;
using Data.Context;
using Data.Entities.FodoEntity;
using Data.Entities.Identity;
using Data.Entities.Log;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Fodo;
using Models.Common;
using Models.Common.Token;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Otp;

namespace Business.Common.TenantDomain;

public class FodoRegistrationService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IUserProfileService ipersonAccessor,
    OtpGeneratorService otpGeneratorService,
    ISmsService smsService)
    : IFodoRegistrationService
{
    public async Task<Result<MessageResponseModel>> RegisterAsync(RegisterFodoRequestModel requestModel)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            // Check if phone number already exists
            var phoneExists = await dbContext.Users
                .AnyAsync(u => u.PhoneNumber == requestModel.MobileNumber && !u.IsDeleted);

            if (phoneExists)
                return Result<MessageResponseModel>.Failed("Mobile number already exists.");

            // Check if email already exists
            if (!string.IsNullOrWhiteSpace(requestModel.Email))
            {
                var emailExists = await dbContext.Users
                    .AnyAsync(u => u.Email == requestModel.Email && !u.IsDeleted);

                if (emailExists)
                    return Result<MessageResponseModel>.Failed("Email already exists.");
            }

            // Check if country id is valid and get country dialing code
            var country = await dbContext.Countries
                .Where(c => c.Id == requestModel.CountryId)
                .FirstOrDefaultAsync();

            if (country == null)
                return Result<MessageResponseModel>.Failed("Invalid country id.");

            var countryDialCode = country.CountryDialingCode;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = $"{countryDialCode}{requestModel.MobileNumber}",
                LockoutEnabled = true,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                PhoneNumber = requestModel.MobileNumber,
                Email = string.IsNullOrWhiteSpace(requestModel.Email) ? null : requestModel.Email.Trim(),
                PhoneCountryId = requestModel.CountryId
            };

            var identityResult = await userManager.CreateAsync(user, requestModel.Password);

            if (!identityResult.Succeeded)
                return Result<MessageResponseModel>.Failed(identityResult.Errors.Select(x => x.Description)
                    .FirstOrDefault());

            var roleResult = await userManager.AddToRoleAsync(user, SystemRoles.Agent);

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result<MessageResponseModel>.Failed(roleResult.Errors.Select(x => x.Description)
                    .FirstOrDefault());
            }

            // Create Fodo entity - TenantId will be set automatically by ApplicationDataContext
            var fodo = new Fodo
            {
                Id = Guid.NewGuid().ToString(),
                FullName = requestModel.FullName?.Trim(),
                UserId = user.Id
            };

            await dbContext.Fodos.AddAsync(fodo);

            await dbContext.SaveChangesAsync();

            var registerOtpCode = await otpGeneratorService.GenerateOtpAsync();

            var dateTime = DateTime.UtcNow;

            await otpGeneratorService.AddOtpAsync(user.Id, registerOtpCode.Item2, OtpType.SignUp, OtpChannel.Sms, SystemModule.Admin, dateTime);

            var smsRequest = new SmsRequest
            {
                Body = $"Welcome to Himalayan Everest Insurance! Your registration is successful. Use this OTP to verify your account: {registerOtpCode.Item1}. The code is valid for 2 minutes. - Thank you for choosing HEI",
                To = [user.PhoneNumber],
                SmsType = SmsType.RegisterCustomer,
            };
            smsService.QueueSms(smsRequest);

            await transaction.CommitAsync();

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Registration successful. Please verify your OTP."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<MessageResponseModel>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<MessageResponseModel>> VerifyFodoOtpAsync(VerifyFodoOtpRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.Username);

        if (user == null || user.IsDeleted)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        var storedOtpCode = await dbContext.UserOtps.Where(x => x.UserId == user.Id
                                                && x.Type == OtpType.SignUp
                                                && x.Channel == OtpChannel.Sms
                                                && !x.IsUsed)
                                                    .FirstOrDefaultAsync();

        if (storedOtpCode == null)
            return Result<MessageResponseModel>.Failed("OTP not found or already used.");

        if (user.PhoneNumberConfirmed)
            return Result<MessageResponseModel>.Failed(ResponseMessage.OtpAlreadVerified);

        var isOtpTimeValid = otpGeneratorService.ValidateOtpTime(storedOtpCode.OTPSentDateTime);

        if (!isOtpTimeValid)
            return Result<MessageResponseModel>.Failed(ResponseMessage.OtpExpired);

        var isOtpValid = await otpGeneratorService.ValidateOtpAsync(storedOtpCode.OTPCode, requestModel.Otp, storedOtpCode.OTPSentDateTime);

        if (!isOtpValid)
            return Result<MessageResponseModel>.Failed(ResponseMessage.OtpNotValid);

        user.PhoneNumberConfirmed = true;
        storedOtpCode.IsUsed = true;

        dbContext.UserOtps.Update(storedOtpCode);

        var roleIds = await userManager.GetRolesAsync(user);

        var tokenModel = tokenService.CreateToken(user, roleIds.ToList());

        var refresh = await tokenService.CreateRefreshToken(user);

        var result = new TokenModel
        {
            AccessToken = tokenModel.Item1,
            AccessTokenExpiryInSeconds = tokenModel.Item2,
            RefreshToken = refresh.Item1,
            RefreshTokenExpiryInSeconds = refresh.Item2
        };

        ipersonAccessor.SetAuthCookiesInClient(result, requestModel.Username);

        await userManager.UpdateAsync(user);
        await dbContext.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel(ResponseMessage.OtpVerified));
    }
}

