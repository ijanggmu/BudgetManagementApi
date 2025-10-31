using System.Net.Http.Headers;
using System.Net.Http.Json;
using Data.Entities.Draft;
using Infrastructure.Common.Khalti;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Common.PolicyPayment;
using Org.BouncyCastle.Asn1.Ocsp;
using SharedKernel.Config;

namespace Business.Common.PolicyPayment.PaymentGateway.Khalti;

// --- Khalti Specific Implementation ---

public class KhaltiPaymentGatewayService(ILogger<KhaltiPaymentGatewayService> logger, IKhaltiApiService khaltiApiService, IOptions<PaymentGatewayOptions> paymentOptions) : IPaymentGatewayService
{
    public async Task<object> InitiatePaymentAsync(PolicyDraft purchase)
    {
        logger.LogInformation($"KhaltiPaymentGatewayService: Initiating payment for purchase ID: {purchase.Id}, Amount: {purchase.NetPremium}");

        var khaltiApiRequestModel = new KhaltiPaymentRequestModel()
        {
            amount = purchase.NetPremium * 100,
            purchase_order_id = purchase.TransactionReference,
            purchase_order_name = $"Buying Policy of {purchase.PortfolioAlias}",
            return_url = paymentOptions.Value.Khalti.ReturnUrl,
            website_url = paymentOptions.Value.Khalti.WebsiteUrl,

        };
        var response = await khaltiApiService.InitialPaymentAsync(khaltiApiRequestModel);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = response.Error?.Content ?? "No error content";
            logger.LogError("Khalti initiation failed | Code: {Code} | Error: {Error}", response.StatusCode, errorBody);
            return null;
        }

        var result = response.Content!;
        logger.LogInformation("Khalti payment initiated | PIDX: {Pidx} | URL: {Url}");

        return result;
    }

    public Task<bool> VerifyPaymentAsync(string transactionReference, decimal amount)
    {
        logger.LogInformation($"KhaltiPaymentGatewayService: Verifying payment for transaction reference: {transactionReference}, Amount: {amount}");
        // In a real Khalti implementation:
        // 1. Make a server-to-server API call to Khalti to verify the transaction status.
        // 2. Parse the response to determine success/failure.
        return Task.FromResult(true); // Placeholder
    }

    public Task<bool> VerifyPaymentAsync(VerifyPaymentRequestModel requestModel)
    {
        throw new NotImplementedException();
    }

    public class KhaltiInitiatePaymentRequest
    {
        public string ReturnUrl { get; set; } = default!;
        public string WebsiteUrl { get; set; } = default!;
        public int Amount { get; set; }
        public string PurchaseOrderId { get; set; } = default!;
        public string PurchaseOrderName { get; set; } = default!;
        public CustomerInfo CustomerInfo { get; set; } = default!;
        public List<AmountBreakdown> AmountBreakdown { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
    }

    public class CustomerInfo
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
    }

    public class AmountBreakdown
    {
        public string Label { get; set; } = default!;
        public int Amount { get; set; }
    }

    public class ProductDetail
    {
        public string Identity { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
    }

    public class KhaltiInitiatePaymentResponse
    {
        public string Pidx { get; set; } = default!;
        public string PaymentUrl { get; set; } = default!;
        public string ExpiresAt { get; set; } = default!;
        public int ExpiresIn { get; set; }



    }
}
