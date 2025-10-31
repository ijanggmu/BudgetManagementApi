using Business.Common.Sms;
using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.Identity;
using Data.Entities.Log;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Otp;

namespace Business.Common.Otp;

public class OptService(ApplicationDataContext dataContext,
    UserManager<ApplicationUser> userManager,
    OtpGeneratorService otpGeneratorService,
    ISmsService smsService) : IOtpService
{
    public async Task<Result<VerifyOtpResponseModel>> VerifyOtpAsync(VerifyOtpRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.Username);

        if (user == null)
            return Result<VerifyOtpResponseModel>.Failed(ResponseMessage.UserNotFound);

        var storedOtpCode = await dataContext.UserOtps
                                            .Include(x => x.User)
                                            .Where(x => x.UserId == user.Id
                                                && x.Type == requestModel.OtpType
                                                && x.Channel == OtpChannel.Sms
                                                && !x.IsUsed)
                                             .FirstOrDefaultAsync();

        var isOtpTimeValid = otpGeneratorService.ValidateOtpTime(storedOtpCode.OTPSentDateTime);

        if (!isOtpTimeValid)
            return Result<VerifyOtpResponseModel>.Failed(ResponseMessage.OtpExpired);

        var isOtpValid = await otpGeneratorService.ValidateOtpAsync(storedOtpCode.OTPCode, requestModel.Otp, storedOtpCode.OTPSentDateTime);

        if (!isOtpValid)
            return Result<VerifyOtpResponseModel>.Failed(ResponseMessage.OtpNotValid);

        storedOtpCode.IsUsed = true;

        var resetToken = Guid.NewGuid().ToString();
        // Store reset token with short expiry (3 minutes)
        var resetExpiry = DateTimeOffset.UtcNow.AddMinutes(3).ToUnixTimeSeconds();
        var resetTokenValue = $"{resetToken}|{resetExpiry}";

        var setResetResult = await userManager.SetAuthenticationTokenAsync(user, "CustomerPortal", requestModel.OtpType.ToString(), resetTokenValue);

        if (!setResetResult.Succeeded)
            return Result<VerifyOtpResponseModel>.Failed("Failed to create authentication token.");

        dataContext.UserOtps.Update(storedOtpCode);
        await dataContext.SaveChangesAsync();

        return Result<VerifyOtpResponseModel>.Success(new VerifyOtpResponseModel(ResponseMessage.OtpVerified, resetToken));
    }
    public async Task<Result<MessageResponseModel>> ResendOtpAsync(ResendOtpRequestModel requestModel)
    {
        var user = await userManager.FindByNameAsync(requestModel.Username);

        if (user is null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        var userOtp = await dataContext.UserOtps
            .Where(x => x.UserId == user.Id &&
                        x.Type == requestModel.OtpType &&
                        x.Channel == OtpChannel.Sms &&
                        !x.IsUsed)
            .OrderByDescending(x => x.OTPSentDateTime)
            .FirstOrDefaultAsync();

        if (userOtp == null)
            return Result<MessageResponseModel>.Failed("Otp is not expired.");

        var isOtpTimeValid = otpGeneratorService.ValidateOtpTime(userOtp.OTPSentDateTime);
        if (isOtpTimeValid)
            return Result<MessageResponseModel>.Failed("Otp is not expired.");

        var (otpValue, otpCode) = await otpGeneratorService.GenerateOtpAsync(user.UserName);

        await otpGeneratorService.AddOtpAsync(
            user.Id,
            otpCode,
            requestModel.OtpType,
            OtpChannel.Sms,
            SystemModule.Customer,
            DateTime.UtcNow
        );
        var smsReqeuest = new SmsRequest();

        if (requestModel.OtpType == OtpType.SignUp)
        {
            smsReqeuest.Body = $"Welcome to Himalayan Everest Insurance! Your registration is successful. Use this OTP to verify your account: {otpValue}. The code is valid for 2 minutes. - Thank you for choosing HEI";
            smsReqeuest.To = [user.PhoneNumber];
        }

        if (requestModel.OtpType == OtpType.ForgotPassword)
        {
            smsReqeuest.Body = $"You requested to reset your password. Use this OTP to proceed: {otpValue}. It is valid for 2 minutes. If you didn’t request this, please ignore this message. - Thank you for choosing HEI\n";
            smsReqeuest.To = [user.PhoneNumber];
        }

        smsService.QueueSms(smsReqeuest);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("OTP Sent Successfully."));
    }

}


