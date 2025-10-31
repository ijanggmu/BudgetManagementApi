using Business.Common.StringCipher;
using Business.Common.Totp;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Constant.Roles;
using SharedKernel.Helper;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.TwoFactor;

public class AdminTwoFactorService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    IUserProfileService ipersonAccessor,
    ITotpService totpService,
    StringCipherService stringCipherService)
    : IAdminTwoFactorService
{
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
            Message = "2FA QR code generated. Scan using your authenticator app."
        });
    }
    public async Task<Result<TwoFaValidateResponseModel>> ValidateTotpCodeAsync(string code)
    {
        var userId = ipersonAccessor.GetUserId();

        var user = await userManager.FindByIdAsync(userId);

        if (user == null || user.IsDisabled || user.IsDeleted)
            return Result<TwoFaValidateResponseModel>.Failed(ResponseMessage.InvalidOtpCode);

        if (string.IsNullOrEmpty(user.TotpSecurityStamp))
            return Result<TwoFaValidateResponseModel>.Failed("2FA not enabled for user!!");

        if (totpService.ValidateTotpCode(user.TotpToken, code))
        {
            if (user.TwoFaSetupStatus != TwoFaSetupStatus.SetupGenerated)
            {
                return Result<TwoFaValidateResponseModel>.Success(new TwoFaValidateResponseModel()
                {
                    Message = "2FA code verified successfully."
                });
            }
            var (pinCodes, pinCodesHash) = await GenerateBackupCodes();
            user.UserTotpBackUpCodes = pinCodesHash;
            user.TwoFaSetupStatus = TwoFaSetupStatus.Verified;
            user.TwoFactorEnabled = true;

            dbContext.Users.Update(user);

            return Result<TwoFaValidateResponseModel>.Success(new TwoFaValidateResponseModel()
            {
                BackupCodes = pinCodes,
                Message = "2FA code verified successfully."
            });
        }
        else
            return Result<TwoFaValidateResponseModel>.Failed(ResponseMessage.Invalid2FACode);
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

        (string[] pinCodes, List<UserTotpBackUpCode> pinCodesHash) = await GenerateBackupCodes();

        agentUser.UserTotpBackUpCodes = pinCodesHash;

        dbContext.Users.Update(agentUser);
        await dbContext.SaveChangesAsync();

        return Result<List<UserTotpBackUpCodeResponseModel>>.Success(pinCodes.Select(x =>
            new UserTotpBackUpCodeResponseModel { CodeHash = x, CreatedOn = pinCodesHash.Select(x => x.CreatedOn).FirstOrDefault().ToUtcString() }).ToList());
    }

    private async Task<(string[] pinCodes, List<UserTotpBackUpCode> pinCodesHash)> GenerateBackupCodes()
    {
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

        return (pinCodes, pinCodesHash);
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
