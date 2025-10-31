namespace SharedKernel.Config;

public class ESewaConfig
{
    public string InitiateUrl { get; set; }
    public string StatusCheckUrl { get; set; }
    public string MerchantId { get; set; }
    public string SecretKey { get; set; }
    public string SignedFieldNamesForInitiation { get; set; }
    public string SignedFieldNamesForCallback { get; set; }
    public string SuccessCallbackUrl { get; set; }
    public string FailureCallbackUrl { get; set; }
}
public class FrontEndUrlOptions
{
    public string CustomerPortal { get; set; }
    public string AgentPortal { get; set; }
}
