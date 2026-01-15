using Business.Common.JobHelper;
using Business.Common.Otp;
using Business.Common.Sms;
using Business.Common.Token;
using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.Identity;
using Data.Entities.Log;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Identity;
using Models.Common;
using Models.Common.Policy.Policy;
using Models.Common.Token;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Otp;

namespace Business.BeemaEdgeApi.Registration;

public class CustomerRegistrationService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IUserProfileService ipersonAccessor,
    HangfireJobHelper hangfireJobHelper,
    OtpGeneratorService otpGeneratorService, ISmsService smsService)
    : ICustomerRegistrationService
{
    public async Task<Result<MessageResponseModel>> RegisterAsync(RegisterCustomerRequestModel requestModel)
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
            var emailExists = await dbContext.Users
                .AnyAsync(u => u.Email == requestModel.Email && u.PhoneNumberConfirmed && !u.IsDeleted);
            if (emailExists)
                return Result<MessageResponseModel>.Failed("Email already exists.");

            // Check if country id is valid and get country dialing code
            var countryDailingCode = await dbContext.Countries
                                         .Where(c => c.Id == requestModel.CountryId)
                                         .Select(x => x.CountryDialingCode)
                                         .FirstOrDefaultAsync();

            if (countryDailingCode == null)
                return Result<MessageResponseModel>.Failed("Invalid country id.");

            var countryDialCode = countryDailingCode;

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

            // Split full name into first, middle, and last names (all trimmed)
            string[] nameParts = (requestModel.FullName ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            string firstName = string.Empty;
            string middleName = string.Empty;
            string lastName = string.Empty;

            if (nameParts.Length == 1)
            {
                firstName = nameParts[0];
            }
            else if (nameParts.Length == 2)
            {
                firstName = nameParts[0];
                lastName = nameParts[1];
            }
            else if (nameParts.Length > 2)
            {
                firstName = nameParts[0];
                lastName = nameParts[^1];
                middleName = string.Join(' ', nameParts.Skip(1).Take(nameParts.Length - 2));
            }

            var customer = new Customer
            {
                FullName = requestModel.FullName?.Trim(),
                UserId = user.Id,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName
            };

            var identityResult = await userManager.CreateAsync(user, requestModel.Password);

            if (!identityResult.Succeeded)
                return Result<MessageResponseModel>.Failed(identityResult.Errors.Select(x => x.Description)
                    .FirstOrDefault());

            var roleResult = await userManager.AddToRoleAsync(user, SystemRoles.FoDo);

            if (!roleResult.Succeeded)
                return Result<MessageResponseModel>.Failed(identityResult.Errors.Select(x => x.Description)
                    .FirstOrDefault());

            await dbContext.Customers.AddAsync(customer);

            await dbContext.SaveChangesAsync();

            var registerOtpCode = await otpGeneratorService.GenerateOtpAsync();

            var dateTime = DateTime.UtcNow;

            await otpGeneratorService.AddOtpAsync(user.Id, registerOtpCode.Item2, SharedKernel.SystemEnum.Otp.OtpType.SignUp, OtpChannel.Sms, SystemModule.Customer, dateTime);

            var smsReqeuest = new SmsRequest
            {
                Body = $"Welcome to Himalayan Everest Insurance! Your registration is successful. Use this OTP to verify your account: {registerOtpCode.Item1}. The code is valid for 2 minutes. - Thank you for choosing HEI",
                To = [customer.User.PhoneNumber],
                SmsType = SmsType.RegisterCustomer,

            };

            smsService.QueueSms(smsReqeuest);

            await transaction.CommitAsync();

            var individualCoreApiRequest = new IndividualCustomerCheckRequestModel(firstName, middleName, lastName, user.PhoneNumber);

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Otp sent successfully."));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }



    public async Task<Result<MessageResponseModel>> VerifyCustomerOtpAsync(VerifyCustomerOtpRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.Username);

        if (user == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        var storedOtpCode = await dbContext.UserOtps.Where(x => x.UserId == user.Id
                                                && x.Type == SharedKernel.SystemEnum.Otp.OtpType.SignUp
                                                && x.Channel == OtpChannel.Sms
                                                && !x.IsUsed)
                                                    .FirstOrDefaultAsync();
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

        return Result<MessageResponseModel>.Success(new MessageResponseModel(ResponseMessage.OtpVerified));
    }

}
