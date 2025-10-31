using SharedKernel.SystemEnum.Otp;

namespace Models.Common.Otp;

public class VerifyOtpRequestModel
{
    public OtpType OtpType { get; set; }
    public SystemModule Module { get; set; }
    public OtpChannel Channel { get; set; }
    public string Otp { get; set; }
    public string Username { get; set; }
}
