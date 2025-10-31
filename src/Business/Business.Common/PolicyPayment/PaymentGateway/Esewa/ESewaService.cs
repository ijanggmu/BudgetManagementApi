using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json; // For System.Text.Json
using System.Web; // For HttpUtility.ParseQueryString if needed, or manual parsing
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Config;

namespace Business.Common.PolicyPayment.PaymentGateway.Esewa;

public class ESewaService(IOptions<PaymentGatewayOptions> paymentGatways, ILogger<ESewaService> logger, HttpClient httpClient)
{
    private readonly ESewaConfig _esewaConfig = paymentGatways?.Value?.Esewa ?? throw new NullReferenceException("PayemntGateWayEsewa:Config");

    /// <summary>
    /// Calculates HMAC-SHA256 signature as required by eSewa.
    /// Input: Concatenated values of parameters in the order specified by signed_field_names.
    /// Output: Base64 encoded hash.
    /// </summary>
    /// <param name="message">The string to sign (e.g., "110,241028,EPAYTEST")</param>
    /// <param name="secretKey">The eSewa Secret Key</param>
    /// <returns>Base64 encoded HMAC-SHA256 signature</returns>
    private string CalculateHMACSHA256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Generates the HTML form for redirecting the user to eSewa's payment page.
    /// This method is for the V2 form-based initiation.
    /// </summary>
    public Dictionary<string, string> GenerateFormDataForPayment(PaymentInitiationRequest requestData, string transactionUuid, string successUrl, string failureUrl)
    {
        // Calculate total_amount as per documentation
        var totalAmount = requestData.Amount + requestData.TaxAmount + requestData.ServiceCharge + requestData.DeliveryCharge;

        // Construct the string for signature generation
        // Parameters should be in the order: total_amount,transaction_uuid,product_code
        // This is explicitly stated in the documentation.
        var messageToSign = $"total_amount={totalAmount},transaction_uuid={transactionUuid},product_code={_esewaConfig.MerchantId}";
        logger.LogInformation($"eSewa Signature Input: '{messageToSign}'");

        var signature = CalculateHMACSHA256(messageToSign, _esewaConfig.SecretKey);
        logger.LogInformation($"eSewa Generated Signature: '{signature}'");

        return new Dictionary<string, string>
        {
            { "amount", requestData.Amount.ToString() },
            { "tax_amount", requestData.TaxAmount.ToString() },
            { "total_amount", totalAmount.ToString() },
            { "transaction_uuid", transactionUuid },
            { "product_code", _esewaConfig.MerchantId},
            { "product_service_charge", requestData.ServiceCharge.ToString() },
            { "product_delivery_charge", requestData.DeliveryCharge.ToString() },
            { "success_url", successUrl },
            { "failure_url", failureUrl },
            { "signed_field_names", _esewaConfig.SignedFieldNamesForInitiation },
            { "signature", signature },
            {"payment_url", _esewaConfig.InitiateUrl}
        };
    }

    /// <summary>
    /// Verifies the incoming eSewa callback response signature.
    /// </summary>
    /// <param name="callbackResponse">The decoded eSewa callback response object.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    public bool VerifyCallbackSignature(ESewaCallbackResponse callbackResponse)
    {
        if (string.IsNullOrEmpty(callbackResponse.SignedFieldNames))
        {
            logger.LogWarning("eSewa callback response missing signed_field_names.");
            return false;
        }

        var signedFields = callbackResponse.SignedFieldNames.Split(',').ToList();
        var messageBuilder = new StringBuilder();

        foreach (var fieldName in signedFields)
        {
            // Get the property name (remove underscores for PascalCase mapping)
            var propertyName = ToPascalCase(fieldName);
            var prop = typeof(ESewaCallbackResponse).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop == null)
            {
                logger.LogWarning($"eSewa callback: Signed field '{fieldName}' not found in response model.");
                return false;
            }

            var value = prop.GetValue(callbackResponse)?.ToString() ?? string.Empty;

            messageBuilder.Append($"{fieldName}={value}");

            if (fieldName != signedFields.Last())
                messageBuilder.Append(",");
        }

        var messageToVerify = messageBuilder.ToString();
        logger.LogInformation($"eSewa Callback Signature Input: '{messageToVerify}'");

        var expectedSignature = CalculateHMACSHA256(messageToVerify, _esewaConfig.SecretKey);
        logger.LogInformation($"eSewa Callback Expected Signature: '{expectedSignature}' vs Received: '{callbackResponse.Signature}'");

        return expectedSignature.Equals(callbackResponse.Signature);
    }

    private string ToPascalCase(string snakeCase)
    {
        return string.Concat(snakeCase.Split('_')
            .Select(word => char.ToUpper(word[0]) + word.Substring(1)));
    }



    /// <summary>
    /// Checks the status of a transaction using eSewa's Status Check API.
    /// </summary>
    public async Task<ESewaStatusCheckResponse> CheckTransactionStatusAsync(string transactionUuid, decimal totalAmount)
    {
        // Construct the query string as per documentation
        // https://rc.esewa.com.np/api/epay/transaction/status/?product_code=EPAYTEST&total_amount=100&transaction_uuid=123
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["product_code"] = _esewaConfig.MerchantId;
        queryString["total_amount"] = totalAmount.ToString();
        queryString["transaction_uuid"] = transactionUuid;

        var requestUrl = $"{_esewaConfig.StatusCheckUrl}?{queryString.ToString()}";

        logger.LogInformation($"eSewa Status Check URL: {requestUrl}");

        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode(); // Throws if not 2xx

        var content = await response.Content.ReadAsStringAsync();
        logger.LogInformation($"eSewa Status Check Response: {content}");

        // Handle potential error messages from eSewa if status code is not 200
        // e.g., {"code": 0, "error_message": "Service is currently unavailable"}
        if (content.Contains("error_message"))
        {
            logger.LogError($"eSewa Status Check API returned an error: {content}");
            // You might want to return a custom error response or throw a specific exception
            return new ESewaStatusCheckResponse { Status = "ERROR_FROM_ESEWA", TransactionUuid = transactionUuid };
        }

        return JsonSerializer.Deserialize<ESewaStatusCheckResponse>(content);
    }
}
