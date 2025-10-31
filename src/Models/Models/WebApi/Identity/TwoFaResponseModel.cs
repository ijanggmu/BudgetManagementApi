namespace Models.BeemaEdgeApi.Identity;

public class TwoFaResponseModel
{
    public string QrCode { get; set; }
    public string Message { get; set; }
}

public class TwoFaValidateResponseModel
{
    public string[] BackupCodes { get; set; }
    public string Message { get; set; }
}
