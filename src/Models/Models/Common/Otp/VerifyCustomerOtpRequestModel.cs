using SharedKernel.SystemEnum.Otp;

namespace Models.Common.Otp;

public class VerifyCustomerOtpRequestModel
{
    public OtpChannel Channel { get; set; }
    public string Otp { get; set; }
    public string Username { get; set; }
}
