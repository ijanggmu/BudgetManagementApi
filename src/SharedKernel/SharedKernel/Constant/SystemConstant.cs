namespace SharedKernel.Constant;
public static class SystemConstant
{
    public const long MaxRequestBodySize = 3670016;

    public const string PasswordField = "password";
    public const string NewPasswordField = "newPassword";
    public const string ConfirmPasswordField = "confirmPassword";
    public const string OtpField = "otp";
    public const string PinField = "pin";
    public const string MPINField = "mpin";


    public const string DefaultSorting = "-CreatedOn";
    public const string CorrelationId = "X-CorrelationId";
    public const string RequestTimeStamp = "X-Timestamp";

}
