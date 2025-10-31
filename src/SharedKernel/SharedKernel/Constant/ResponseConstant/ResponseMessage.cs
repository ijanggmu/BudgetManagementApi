namespace SharedKernel.Constant.ResponseConstant;
public static class ResponseMessage
{
    public const string UnknownError = "Something Went Wrong !!!";
    public const string NewAndConfirmPasswordNotSame = "New password and confirm password must be same.";
    public const string InvalidOldPassword = "Invalid your current password.";
    public const string NewAndOldPasswordSame = "New password and old password cannot be same.";

    public const string Invalid2FACode = "Invalid 2FA code.";
    public const string InvalidOtpCode = "Invalid opt code.";

    public static string UserNotFound = "User not Found.";
    public static string OtpVerified = "Otp verified Successfully";
    public static string Used2FACode;
    public static string OtpNotVerified = "Otp not verified.";
    public static string OtpExpired = "Otp expired.";
    public static string OtpNotValid = "Otp not valid.";
    public static string OtpAlreadVerified = "Otp already verified.";
}
public static class ErrorCodeConstant
{
    public const int OtpNotVerified = 4031;
}
