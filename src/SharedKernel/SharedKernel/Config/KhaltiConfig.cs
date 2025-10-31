namespace SharedKernel.Config;

public class KhaltiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
public class PaymentGatewayOptions
{
    public KhaltiConfig Khalti { get; set; }
    public ESewaConfig Esewa { get; set; }
}
