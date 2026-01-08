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
using Models.Common;
using Models.Common.Token;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Constant.Roles;
using SharedKernel.Helper;
using SharedKernel.Operation;
using Microsoft.AspNetCore.Http;
using Data.Entities.Tenant;
using Business.Common.File;

namespace Business.AdminPortalApi.Auth;

public class AdminAuthService(ApplicationDataContext dbContext,
UserManager<ApplicationUser> userManager,
SignInManager<ApplicationUser> signInManager,
ITokenService tokenService,
IUserProfileService ipersonAccessor,
ITotpService totpService,
StringCipherService stringCipherService,
IFileService fileService) : IAdminAuthService
{
    public async Task<Result<LoginAdminResponseModel>> LoginAsync(AdminLoginRequestModel requestModel, CancellationToken ct)
    {

        var user = await dbContext.Users
                .Where(x => x.UserName == requestModel.Username
                && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

        if (user == null)
            return Result<LoginAdminResponseModel>.Failed("Username or password is invalid.");


        //var isAdminRoledUser = await (from ur in dbContext.UserRoles
        //                              join r in dbContext.Roles on ur.RoleId equals r.Id
        //                              join u in dbContext.Users on ur.UserId equals u.Id
        //                              where ur.UserId == user.Id
        //                              && !ur.IsDeleted
        //                              select ur)
        //                              .AnyAsync(ct);

        //if (!isAdminRoledUser)
        //    return Result<LoginAdminResponseModel>.Failed("Username or password is invalid.");

        var identityResult =
             await signInManager.CheckPasswordSignInAsync(user, requestModel.Password, lockoutOnFailure: false);

        if (identityResult == SignInResult.Failed)
        {
            user.AccessFailedCount++;
            await userManager.UpdateAsync(user);
        }

        if (identityResult.IsLockedOut)
            return Result<LoginAdminResponseModel>.Failed("Too many login attempts. Please try again in a while.");

        if (user.IsDisabled)
            return Result<LoginAdminResponseModel>.Failed("User is disabled. Please contact administrator.");

        if (identityResult.IsNotAllowed)
            return Result<LoginAdminResponseModel>.Failed("User is not allowed to login. Please contact administrator.");

        if (!identityResult.Succeeded)
            return Result<LoginAdminResponseModel>.Failed("Username or password is invalid.");

        var responseModel = new LoginAdminResponseModel
        {
            IsTwoFactorEnabled = user.TwoFactorEnabled
        };

        //if (!user.PhoneNumberConfirmed)
        //    return Result<LoginCustomerResponseModel>.Failed(ResponseMessage.OtpNotVerified, errorCode: ErrorCodeConstant.OtpNotVerified);


        if ((user.EmailConfirmed || user.PhoneNumberConfirmed) && user.TwoFactorEnabled)
        {
            var token = userManager.PasswordHasher.HashPassword(user, Guid.NewGuid().ToString());
            user.TotpToken = token;
            user.TotpTokenEnd = DateTime.UtcNow.AddHours(1);

            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(ct);
            responseModel.Token = token;
        }

        if ((user.EmailConfirmed || user.PhoneNumberConfirmed) && !user.TwoFactorEnabled)
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

            // Include branding if user has tenant
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                var tenant = await dbContext.Tenants
                    .Include(t => t.Branding)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == user.TenantId, cancellationToken: ct);
                if (tenant?.Branding != null)
                {
                    responseModel.Branding = new BrandingResponseModel
                    {
                        TenantId = tenant.Branding.TenantId,
                        LogoSignedUrl = !string.IsNullOrEmpty(tenant.Branding.LogoUrl)?await fileService.GetFilePresignedUrlAsync(tenant.Branding.LogoUrl) :tenant.Branding.LogoUrl,
                        PaletteJson = tenant.Branding.PaletteJson,
                        TypographyJson = tenant.Branding.TypographyJson,
                        Version = tenant.Branding.Version
                    };
                }
                return Result<LoginAdminResponseModel>.Success(responseModel, statusCode: HttpStatusCode.OK);

            }

            return Result<LoginAdminResponseModel>.Success(responseModel, statusCode: HttpStatusCode.NoContent);

        }

        return Result<LoginAdminResponseModel>.Success(responseModel);

    }
    public async Task<Result<MessageResponseModel>> Login2FaAsync(Verify2FaAdminRequestModel requestModel, CancellationToken ct)
    {
        var user = await dbContext.Users
            .Where(x => x.TotpToken == requestModel.Token)
            .FirstOrDefaultAsync(ct);

        if (user == null || user.IsDeleted || user.TotpTokenEnd < DateTimeOffset.UtcNow)
            return Result<MessageResponseModel>.Failed(ResponseMessage.Invalid2FACode);

        if (!totpService.ValidateTotpCode(user.TotpToken, requestModel.Code))
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
            .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

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
        await dbContext.SaveChangesAsync(ct);

        ipersonAccessor.SetAuthCookiesInClient(result, user.UserName);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Login successfully."), statusCode: HttpStatusCode.NoContent);
    }
    public async Task<Result<MessageResponseModel>> RefreshTokenAsync(CancellationToken ct)
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

            // Refresh token rotation: Invalidate old token before creating new one
            // This prevents token reuse attacks
            userInfo.User.RefreshToken = null;
            userInfo.User.RefreshTokenExpiryDateTime = null;
            dbContext.Users.Update(userInfo.User);
            await dbContext.SaveChangesAsync();

            var roleNames = await (from role in dbContext.Roles
                                   join userRoles in dbContext.UserRoles
                                       on role.Id equals userRoles.RoleId
                                   where !role.IsDeleted && userRoles.UserId == userInfo.User.Id
                                   select role.Name
                ).ToListAsync();

            var newAccessToken = tokenService.CreateToken(userInfo.User, roleNames);
            var newRefreshToken = await tokenService.CreateRefreshToken(userInfo.User);

            await transaction.CommitAsync(ct);

            var result = new TokenModel
            {
                AccessToken = newAccessToken.Item1,
                AccessTokenExpiryInSeconds = newAccessToken.Item2,
                RefreshToken = newRefreshToken.Item1,
                RefreshTokenExpiryInSeconds = newRefreshToken.Item2
            };

            ipersonAccessor.SetAuthCookiesInClient(result, username);
            return Result<MessageResponseModel>.Success(new MessageResponseModel("RefreshTokenGeneratedSuccessfully"), statusCode: HttpStatusCode.NoContent);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result<MessageResponseModel>> LogoutAsync(HttpResponse response, CancellationToken ct)
    {
        var refreshToken = ipersonAccessor.GetRefreshToken();
        var username = ipersonAccessor.GetUsername();

        if (!string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(username))
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.UserName == username, ct);

            if (user != null)
            {
                // Revoke refresh token by clearing it
                user.RefreshToken = null;
                user.RefreshTokenExpiryDateTime = null;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync(ct);
            }
        }

        ipersonAccessor.RemoveAuthCookies(response);
        return Result<MessageResponseModel>.Success(new MessageResponseModel("Logged out successfully."));
    }
}
