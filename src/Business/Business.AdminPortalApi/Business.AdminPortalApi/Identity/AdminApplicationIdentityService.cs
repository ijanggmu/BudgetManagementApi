using System.Net;
using Business.Common.Otp;
using Business.Common.StringCipher;
using Business.Common.Token;
using Business.Common.Totp;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Common;
using Models.Common.Token;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Constant.Roles;
using SharedKernel.Helper;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Otp;

namespace Business.AdminPortalApi.Identity;

public class AdminApplicationIdentityService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IUserProfileService ipersonAccessor,
    ITotpService totpService,
    StringCipherService stringCipherService,
    OtpGeneratorService otpGeneratorService)
    : IAdminApplicationIdentityService
{
    public async Task<Result<LoginCustomerResponseModel>> LoginAsync(IndividualLoginRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.Username);

        if (user == null || user.IsDeleted)
            return Result<LoginCustomerResponseModel>.Failed("Username or password is invalid.");

        var identityResult =
            await signInManager.CheckPasswordSignInAsync(user, requestModel.Password, lockoutOnFailure: false);

        if (identityResult == SignInResult.Failed)
        {
            user.AccessFailedCount++;
            await userManager.UpdateAsync(user);
        }

        if (identityResult.IsLockedOut)
            return Result<LoginCustomerResponseModel>.Failed("Too many login attempts. Please try again in a while.");

        if (user.IsDisabled)
            return Result<LoginCustomerResponseModel>.Failed("User is disabled. Please contact administrator.");

        if (identityResult.IsNotAllowed)
            return Result<LoginCustomerResponseModel>.Failed("User is not allowed to login. Please contact administrator.");

        if (!identityResult.Succeeded)
            return Result<LoginCustomerResponseModel>.Failed("Username or password is invalid.");

        var responseModel = new LoginCustomerResponseModel
        {
            IsTwoFactorEnabled = user.TwoFactorEnabled
        };

        if (!user.PhoneNumberConfirmed)
            return Result<LoginCustomerResponseModel>.Failed(ResponseMessage.OtpNotVerified, errorCode: ErrorCodeConstant.OtpNotVerified);

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
        var user = await dbContext.Users.Where(x => x.TotpToken == requestModel.Token).FirstOrDefaultAsync();

        if (user == null || user.IsDeleted || user.TotpTokenEnd < DateTimeOffset.UtcNow)
            return Result<MessageResponseModel>.Failed(ResponseMessage.Invalid2FACode);

        if (!totpService.ValidateTotpCode(requestModel.Token, requestModel.Code))
        {
            bool backupCodeFound = false;
            bool backupCodeUsed = false;

            if (user.UserTotpBackUpCodes != null)
            {
                var backupCodes = user.UserTotpBackUpCodes;
                foreach (var backUpCode in backupCodes)
                {
                    var codeDb = await stringCipherService.DecryptAsync(backUpCode.CodeHash);

                    if (backUpCode.IsUsed)
                    {
                        if (codeDb == requestModel.Code)
                            backupCodeUsed = true;

                        continue;
                    }

                    if (codeDb == requestModel.Code)
                    {
                        backUpCode.IsUsed = true;
                        backUpCode.UsedOn = DateTimeOffset.UtcNow;

                        backupCodeFound = true;
                        user.UserTotpBackUpCodes = backupCodes;

                        break;
                    }
                }
            }

            if (backupCodeUsed)
                return Result<MessageResponseModel>.Failed(ResponseMessage.Used2FACode);

            if (!backupCodeFound)
                return Result<MessageResponseModel>.Failed(ResponseMessage.Invalid2FACode);
        }

        var roleIds = await userManager.GetRolesAsync(user);

        if (!await dbContext.Roles.AnyAsync(x => roleIds.Contains(x.Name) && x.RoleType == SystemRoles.Admin))
            return Result<MessageResponseModel>.Failed(ResponseMessage.Invalid2FACode);

        var Admin = await dbContext.Customers
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        if (Admin == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.Invalid2FACode);

        var accessToken = tokenService.CreateToken(user, roleIds.ToList());

        var refreshToken = await tokenService.CreateRefreshToken(user);

        var result = new TokenModel
        {
            AccessToken = accessToken.Item1,
            AccessTokenExpiryInSeconds = accessToken.Item2,
            RefreshToken = refreshToken.Item1,
            RefreshTokenExpiryInSeconds = refreshToken.Item2
        };

        if (user.UserTotpBackUpCodes is null)
        {
            var pincodes = PincodeGeneratorHelper.GenerateMultiplePincodes(6, 10);
            var pincodesHash = new List<UserTotpBackUpCode>();
            foreach (var pincode in pincodes)
            {
                pincodesHash.Add(new UserTotpBackUpCode
                {
                    CodeHash = await stringCipherService.EncryptAsync(pincode),
                    CreatedOn = DateTimeOffset.UtcNow
                });
            }

            user.UserTotpBackUpCodes = pincodesHash;
        }

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        ipersonAccessor.SetAuthCookiesInClient(result, user.UserName);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Login successfully."), statusCode: HttpStatusCode.NoContent);
    }
    public async Task<Result<MessageResponseModel>> VerifyAdminOtpAsync(VerifyCustomerOtpRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.Username);

        if (user == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        var storedOtpCode = await dbContext.UserOtps.Where(x => x.UserId == user.Id
                                                && x.Type == OtpType.SignUp
                                                //&& x.Channel == requestModel.Channel
                                                && !x.IsUsed)
                      .FirstOrDefaultAsync();

        var isOtpValid = await otpGeneratorService.ValidateOtpAsync(requestModel.Otp, storedOtpCode.OTPCode, storedOtpCode.OTPSentDateTime);

        if (!isOtpValid)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        //if (requestModel.Channel == OtpChannel.Sms)
        //{
        //    user.PhoneNumberConfirmed = true;
        //}
        //else if (requestModel.Channel == OtpChannel.Email)
        //{
        //    user.EmailConfirmed = true;
        //}

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
    public async Task<Result<MessageResponseModel>> RefreshTokenAsync()
    {
        var refreshToken = ipersonAccessor.GetRefreshToken();

        if (refreshToken == null)
            return Result<MessageResponseModel>.Failed("Failed to refresh token.");

        var username = ipersonAccessor.GetUsername();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var userInfo = await (from user in dbContext.Users
                                  where user.RefreshToken == refreshToken && user.UserName == username
                                  select new { User = user, user.RefreshToken, user.RefreshTokenExpiryDateTime }).FirstOrDefaultAsync();

            if (userInfo == null || userInfo.RefreshTokenExpiryDateTime <= DateTime.UtcNow)
                return Result<MessageResponseModel>.Failed("Refresh token already expired.");

            if (userInfo.User == null || userInfo.RefreshToken != refreshToken)
                return Result<MessageResponseModel>.Failed("Invalid refresh token.");


            var roleNames = await (from role in dbContext.Roles
                                   join userRoles in dbContext.UserRoles
                                       on role.Id equals userRoles.RoleId
                                   where !role.IsDeleted && userRoles.UserId == userInfo.User.Id
                                   select role.Name
                ).ToListAsync();

            var newAccessToken = tokenService.CreateToken(userInfo.User, roleNames);
            var newRefreshToken = await tokenService.CreateRefreshToken(userInfo.User);

            await transaction.CommitAsync();

            var result = new TokenModel
            {
                AccessToken = newAccessToken.Item1,
                AccessTokenExpiryInSeconds = newAccessToken.Item2,
                RefreshToken = newRefreshToken.Item1,
                RefreshTokenExpiryInSeconds = newRefreshToken.Item2
            };

            ipersonAccessor.SetAuthCookiesInClient(result, username);
            return Result<MessageResponseModel>.Success(new MessageResponseModel("RefreshTokenGeneratedSuccessfully"));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<Result<MessageResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (requestModel.ConfirmPassword != requestModel.NewPassword)
            return Result<MessageResponseModel>.Failed(ResponseMessage.NewAndConfirmPasswordNotSame);

        var isOldPasswordCorrect =
            await userManager.CheckPasswordAsync(user, requestModel.OldPassword);

        if (!isOldPasswordCorrect)
            return Result<MessageResponseModel>.Failed(ResponseMessage.InvalidOldPassword);

        var isOldPasswordSame =
            await userManager.CheckPasswordAsync(user, requestModel.NewPassword);

        if (isOldPasswordSame)
            return Result<MessageResponseModel>.Failed(ResponseMessage.NewAndOldPasswordSame);


        var result = await userManager.ChangePasswordAsync(user, requestModel.OldPassword,
            requestModel.NewPassword);

        if (!result.Succeeded)
            return Result<MessageResponseModel>.Failed(result.Errors.Select(x => x.Description).FirstOrDefault());

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Password changed successfully."));
    }
    public Task<Result<MessageResponseModel>> ForgetPasswordAsync(ForgetPasswordRequestModel requestModel)
    {
        throw new NotImplementedException();
    }
    public async Task<Result<MessageResponseModel>> SetPasswordAsync(ChangePasswordRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();
        var user = await userManager.FindByIdAsync(userId);

        if (user == null || user.IsDisabled || user.IsDeleted)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        // Check if the user already has a password
        var hasPassword = await userManager.HasPasswordAsync(user);
        if (hasPassword)
            return Result<MessageResponseModel>.Failed("Password already set. Use change password instead.");

        if (requestModel.NewPassword != requestModel.ConfirmPassword)
            return Result<MessageResponseModel>.Failed(ResponseMessage.NewAndConfirmPasswordNotSame);

        var result = await userManager.AddPasswordAsync(user, requestModel.NewPassword);

        if (!result.Succeeded)
            return Result<MessageResponseModel>.Failed(result.Errors.Select(x => x.Description).FirstOrDefault());

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Password set successfully."));
    }
    public async Task<Result<UserProfileResponseModel>> GetProfileAsync()
    {
        var userId = ipersonAccessor.GetUserId();

        var Admin = await dbContext.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (Admin == null)
            return Result<UserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

        var profile = new UserProfileResponseModel
        {
            FullName = Admin.FullName,
            Email = Admin.User.Email,
            PhoneNumber = Admin.User.PhoneNumber
        };

        return Result<UserProfileResponseModel>.Success(profile);
    }
    public async Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();

        var Admin = await dbContext.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (Admin == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            Admin.FullName = requestModel.FullName;

        dbContext.Customers.Update(Admin);
        dbContext.Users.Update(Admin.User);
        await dbContext.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Profile updated successfully."));
    }
    public async Task<Result<TwoFaResponseModel>> Set2FaAsync()
    {
        var userId = ipersonAccessor.GetUserId();
        var roleId = ipersonAccessor.GetRoleId();
        var roleIds = roleId.Split(",");

        var user = await userManager.FindByIdAsync(userId);

        if (!await dbContext.Roles.AnyAsync(x => roleIds.Contains(x.Name) && x.RoleType == SystemRoles.Admin))
            return Result<TwoFaResponseModel>.Failed(ResponseMessage.UserNotFound);

        var qrCode = await totpService.GenerateTotpQrCode(user);

        return Result<TwoFaResponseModel>.Success(new TwoFaResponseModel
        {
            QrCode = qrCode,
            Message = "2 FA set successfully."
        });
    }
    public async Task<Result<MessageResponseModel>> ValidateTotpCodeAsync(string code)
    {
        var userLogged = ipersonAccessor.GetUser();
        var userId = userLogged.UserId;
        var user = await userManager.FindByIdAsync(userId);

        if (user == null || user.IsDisabled || user.IsDeleted)
            return Result<MessageResponseModel>.Failed(ResponseMessage.InvalidOtpCode);

        if (!user.TwoFactorEnabled)
            return Result<MessageResponseModel>.Failed("2FA not enabled for user!!");

        if (totpService.ValidateTotpCode(user.TotpToken, code))
        {
            //user.IsRegistrationCompleted = true;
            dbContext.Users.Update(user);
            return Result<MessageResponseModel>.Success(new MessageResponseModel("2FA code verified successfully."));
        }
        else
            return Result<MessageResponseModel>.Failed(ResponseMessage.Invalid2FACode);
    }
    public async Task<Result<MessageResponseModel>> Disable2FaAsync()
    {
        var userLogged = ipersonAccessor.GetUser();

        var userId = userLogged.UserId;
        var user = await userManager.FindByIdAsync(userId);

        if (user == null || user.IsDisabled || user.IsDeleted)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (!user.TwoFactorEnabled)
            return Result<MessageResponseModel>.Failed("2FA not enabled for user!!");

        await totpService.Disable2FaAsync(user);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("2FA disabled successfully."));
    }
    public async Task<Result<List<UserTotpBackUpCodeResponseModel>>> Generate2FaBackUpCodesAsync()
    {
        var userLogged = ipersonAccessor.GetUser();

        var userId = userLogged.UserId;

        var agentUser = await dbContext.Customers
            .Include(x => x.User)
            .Where(x => x.User.Id == userId && !x.User.IsDeleted &&
                        x.User.TotpSecurityStamp != null)
            .Select(x => x.User)
            .FirstOrDefaultAsync();

        if (agentUser == null)
            return Result<List<UserTotpBackUpCodeResponseModel>>.Failed("Admin not found or 2 FA not enabled.");

        var pinCodes = PincodeGeneratorHelper.GenerateMultiplePincodes(6, 10);

        var timeNow = DateTimeOffset.UtcNow;
        var pinCodesHash = new List<UserTotpBackUpCode>();
        foreach (var pinCode in pinCodes)
        {
            pinCodesHash.Add(new UserTotpBackUpCode
            {
                CodeHash = await stringCipherService.EncryptAsync(pinCode),
                CreatedOn = timeNow
            });
        }

        agentUser.UserTotpBackUpCodes = pinCodesHash;

        dbContext.Users.Update(agentUser);
        await dbContext.SaveChangesAsync();

        return Result<List<UserTotpBackUpCodeResponseModel>>.Success(pinCodes.Select(x =>
            new UserTotpBackUpCodeResponseModel { CodeHash = x, CreatedOn = timeNow.ToUtcString() }).ToList());
    }
    public async Task<Result<List<UserTotpBackUpCodeResponseModel>>> GetAll2FaBackUpCodesAsync()
    {
        var userLogged = ipersonAccessor.GetUser();
        var userId = userLogged.UserId;

        var backUpCodesHashed = await dbContext.Customers
            .Include(x => x.User)
            .Where(x => x.User.Id == userId && !x.User.IsDeleted &&
                        x.User.TotpSecurityStamp != null)
            .Select(x => x.User.UserTotpBackUpCodes)
            .FirstOrDefaultAsync();

        if (backUpCodesHashed == null)
            return Result<List<UserTotpBackUpCodeResponseModel>>.Failed("Admin not found or 2 FA not enabled.");

        var backupCodes = new List<UserTotpBackUpCodeResponseModel>();
        foreach (var pinCode in backUpCodesHashed)
        {
            backupCodes.Add(new UserTotpBackUpCodeResponseModel
            {
                CodeHash = await stringCipherService.DecryptAsync(pinCode.CodeHash),
                CreatedOn = pinCode.CreatedOn.ToUtcString(),
                IsUsed = pinCode.IsUsed,
                UsedOn = pinCode.UsedOn.ToUtcString()
            });
        }

        return Result<List<UserTotpBackUpCodeResponseModel>>.Success(backupCodes);
    }
}
