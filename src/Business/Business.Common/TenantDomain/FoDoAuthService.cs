using System.Net;
using Business.Common.Otp;
using Business.Common.Sms;
using Business.Common.Token;
using Business.Common.Totp;
using Data.Context;
using Data.Entities.FodoEntity;
using Data.Entities.Identity;
using Data.Entities.Log;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Fodo;
using Models.BeemaEdgeApi.Identity;
using Models.Common;
using Models.Common.Token;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Otp;

namespace Business.Common.TenantDomain;

public class FodoAuthService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IUserProfileService ipersonAccessor,
    ITotpService totpService,
    OtpGeneratorService otpGeneratorService,
    ISmsService smsService)
    : IFodoAuthService
{
    public async Task<Result<LoginCustomerResponseModel>> LoginAsync(AgentLoginRequestModel requestModel)
    {
        var fodo = await dbContext.Fodos
            .Include(x => x.User)
            .Where(x => x.User.UserName == requestModel.Username)
            .FirstOrDefaultAsync();

        if (fodo == null || fodo.User == null || fodo.User.IsDeleted)
            return Result<LoginCustomerResponseModel>.Failed("Username or password is invalid.");

        var user = fodo.User;

        var identityResult =
            await signInManager.CheckPasswordSignInAsync(user, requestModel.Password, lockoutOnFailure: false);

        if (identityResult == SignInResult.Failed)
            return Result<LoginCustomerResponseModel>.Failed("Username or password is invalid.");

        var responseModel = new LoginCustomerResponseModel
        {
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsTwoFactorEnabled = user.TwoFactorEnabled
        };

        if (!user.PhoneNumberConfirmed)
        {
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
        }

        if (user.PhoneNumberConfirmed && user.TwoFactorEnabled)
        {
            var token = userManager.PasswordHasher.HashPassword(user, Guid.NewGuid().ToString());
            user.TotpToken = token;
            user.TotpTokenEnd = DateTime.UtcNow.AddHours(1);

            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            responseModel.Token = token;
        }

        if (user.PhoneNumberConfirmed && !user.TwoFactorEnabled)
        {
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

            return Result<LoginCustomerResponseModel>.Success(responseModel, statusCode: HttpStatusCode.NoContent);
        }

        return Result<LoginCustomerResponseModel>.Success(responseModel);
    }

    public async Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaCustomerRequestModel requestModel)
    {
        var agent = await dbContext.Fodos.Include(x => x.User)
            .Where(x => x.User.TotpToken == requestModel.Token)
            .FirstOrDefaultAsync();

        if (agent == null || agent.User == null)
            return Result<MessageResponseModel>.Failed("Invalid token.");

        var user = agent.User;

        if (user.TotpTokenEnd < DateTime.UtcNow)
            return Result<MessageResponseModel>.Failed("Token expired.");

        var isValid = totpService.ValidateTotpCode(requestModel.Token, requestModel.Code);

        if (!isValid)
            return Result<MessageResponseModel>.Failed("Invalid code.");

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

        ipersonAccessor.SetAuthCookiesInClient(result, user.UserName);

        user.TotpToken = null;
        user.TotpTokenEnd = null;

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Login successful."));
    }

    public async Task<Result<MessageResponseModel>> RefreshTokenAsync()
    {
        var userId = ipersonAccessor.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

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

        ipersonAccessor.SetAuthCookiesInClient(result, user.UserName);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Token refreshed successfully."));
    }

    public Result<MessageResponseModel> Logout(HttpResponse response)
    {
        ipersonAccessor.RemoveAuthCookies(response);
        return Result<MessageResponseModel>.Success(new MessageResponseModel("Logout successful."));
    }
}

