using System.Globalization;
using Data.Entities.Draft;
using Microsoft.Extensions.Logging;

namespace Business.Common.PolicyPayment.PaymentGateway.ConnectIPS;

// --- ConnectIPS Specific Implementation ---

public class ConnectIPSPaymentGatewayService /*: IPaymentGatewayService*/
{
    private readonly ILogger<ConnectIPSPaymentGatewayService> _logger;
    // ConnectIPS configuration and HttpClient would be injected here
    // private readonly ConnectIPSConfig _config;
    // private readonly HttpClient _httpClient;

    public ConnectIPSPaymentGatewayService(ILogger<ConnectIPSPaymentGatewayService> logger /*, IConfiguration configuration, HttpClient httpClient */)
    {
        _logger = logger;
        // _config = configuration.GetSection("PaymentGateways:ConnectIPS").Get<ConnectIPSConfig>() ?? throw new InvalidOperationException("ConnectIPS configuration is missing.");
        // _httpClient = httpClient;
    }

    public async Task<object> InitiatePaymentAsync(PolicyDraft purchase)
    {
        if (purchase == null)
            throw new ArgumentNullException(nameof(purchase));

        if (string.IsNullOrWhiteSpace(purchase.TransactionReference))
            throw new ArgumentException("TransactionReference is required.", nameof(purchase.TransactionReference));

        if (purchase.NetPremium <= 0)
            throw new ArgumentException("NetPremium must be greater than zero.", nameof(purchase.NetPremium));

        _logger.LogInformation("ConnectIPSPaymentGatewayService: Initiating payment for Purchase ID: {PurchaseId}, Amount: {Amount}", purchase.Id, purchase.NetPremium);

        var queryParams = new Dictionary<string, string>
    {
        { "total", purchase.NetPremium.ToString("F2", CultureInfo.InvariantCulture) },
        { "txnId", purchase.TransactionReference }
    };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var paymentUrl = $"https://mockconnectips.com/pay?{queryString}";

        return new Dictionary<string, string>
    {
        { "gateway", "connectIps" },
        { "url", paymentUrl }
    };
    }


    public Task<bool> VerifyPaymentAsync(string transactionReference, decimal amount)
    {
        _logger.LogInformation($"ConnectIPSPaymentGatewayService: Verifying payment for transaction reference: {transactionReference}, Amount: {amount}");
        // In a real ConnectIPS implementation:
        // 1. Make a server-to-server API call to verify the transaction status.
        // 2. Validate signatures if required by ConnectIPS.
        return Task.FromResult(true); // Placeholder
    }
}

