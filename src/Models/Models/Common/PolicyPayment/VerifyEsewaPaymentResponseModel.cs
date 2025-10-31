namespace Models.Common.PolicyPayment;

public class VerifyEsewaPaymentResponseModel(string redirectUrl)
{
    public string RedirectUrl { get; set; } = redirectUrl;

}

public class VerifyKhaltiPaymentResponseModel(string redirectUrl)
{
    public string RedirectUrl { get; set; } = redirectUrl;

}
