using Business.Common.Otp;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Otp;

namespace Business.AdminPortalApi.AdminPassword;

public class AdminPasswordService(ApplicationDataContext dbContext,
 UserManager<ApplicationUser> userManager,
 IUserProfileService ipersonAccessor,
 OtpGeneratorService otpGeneratorService) : IAdminPasswordService
{
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

    public async Task<Result<MessageResponseModel>> ForgetPasswordAsync(ForgetPasswordRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.UserName);

        if (user == null || user.IsDeleted || user.IsDisabled)
            return Result<MessageResponseModel>.Failed("User not found.");

        var registerOtpCode = await otpGeneratorService.GenerateOtpAsync();

        var dateTime = DateTime.UtcNow;

        await otpGeneratorService.AddOtpAsync(user.Id, registerOtpCode.Item2, OtpType.ForgotPassword, OtpChannel.Sms, SystemModule.Admin, dateTime);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("OTP sent successfully."));
    }

    public async Task<Result<MessageResponseModel>> SetPasswordAsync(ChangePasswordRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();
        var user = await userManager.FindByIdAsync(userId);

        if (user == null || user.IsDisabled || user.IsDeleted)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

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
    public async Task<Result<MessageResponseModel>> ResetPasswordWithOtpAsync(ResetPasswordRequestModel requestModel)
    {
        var user = await userManager.FindByEmailAsync(requestModel.UserName);

        if (user == null || user.IsDeleted || user.IsDisabled)
            return Result<MessageResponseModel>.Failed("User not found.");

        var isOtpValid = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ForgotPasswordOTP", requestModel.Token);
        if (!isOtpValid)
            return Result<MessageResponseModel>.Failed("Invalid or expired OTP.");

        if (requestModel.NewPassword != requestModel.ConfirmPassword)
            return Result<MessageResponseModel>.Failed("Passwords do not match.");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, requestModel.NewPassword);

        if (!result.Succeeded)
            return Result<MessageResponseModel>.Failed(result.Errors.FirstOrDefault()?.Description);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Password reset successfully."));
    }
}
