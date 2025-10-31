using Business.Common.Otp;
using Business.Common.Sms;
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

namespace Business.BeemaEdgeApi.CustomerPassword;

public class CustomerPasswordService(
 UserManager<ApplicationUser> userManager,
 IUserProfileService ipersonAccessor,
 OtpGeneratorService otpGeneratorService,
 ISmsService smsService) : ICustomerPasswordService
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

        await otpGeneratorService.AddOtpAsync(user.Id, registerOtpCode.Item2, OtpType.ForgotPassword, OtpChannel.Sms, SystemModule.Customer, dateTime);

        var smsRequest = new SmsRequest()
        {
            Body = $"You requested to reset your password. Use this OTP to proceed: {registerOtpCode.Item1}. It is valid for 2 minutes. If you didn’t request this, please ignore this message. - Thank you for choosing HEI\n",
            To = [user.PhoneNumber],
        };
        smsService.QueueSms(smsRequest);
        return Result<MessageResponseModel>.Success(new MessageResponseModel("OTP sent successfully."));
    }

    public async Task<Result<MessageResponseModel>> SetPasswordAsync(SetPasswordRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.UserName);

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
    public async Task<Result<MessageResponseModel>> ResetPasswordAsync(ResetPasswordRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.UserName);

        if (user == null || user.IsDeleted || user.IsDisabled)
            return Result<MessageResponseModel>.Failed("User not found.");

        var storedResetTokenValue = await userManager.GetAuthenticationTokenAsync(user, "CustomerPortal", nameof(OtpType.ForgotPassword));

        if (string.IsNullOrEmpty(storedResetTokenValue))
            return Result<MessageResponseModel>.Failed("Reset token not found or expired");

        var parts = storedResetTokenValue.Split('|');
        if (parts.Length != 2)
            return Result<MessageResponseModel>.Failed("Malformed reset token");

        var storedResetToken = parts[0];
        var expiryUnix = long.Parse(parts[1]);

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiryUnix)
            return Result<MessageResponseModel>.Failed("Reset token expired");

        if (storedResetToken != requestModel.Token)
            return Result<MessageResponseModel>.Failed("Invalid reset token");

        // Remove old password and set new one
        var removePasswordResult = await userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
            return Result<MessageResponseModel>.Failed("Failed to remove old password");

        var addPasswordResult = await userManager.AddPasswordAsync(user, requestModel.NewPassword);
        if (!addPasswordResult.Succeeded)
            return Result<MessageResponseModel>.Failed("Failed to set new password");

        await userManager.RemoveAuthenticationTokenAsync(user, "CustomerPortal", nameof(OtpType.ForgotPassword));

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Password reset successfully."));
    }
}
