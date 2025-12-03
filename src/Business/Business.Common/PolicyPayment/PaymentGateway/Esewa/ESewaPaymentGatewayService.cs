using System.Text;
using System.Text.Json;
using Business.Common.JobHelper;
using Business.Common.JobHelper.CustomerPolicyJob;
using Data.Context;
using Data.Entities.PolicyE2e;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Common.PolicyPayment;
using SharedKernel.Config;
using SharedKernel.SystemEnum.Payment;

namespace Business.Common.PolicyPayment.PaymentGateway.Esewa;

public class ESewaPaymentGatewayService(
    ESewaService eSewaService,
    IOptions<PaymentGatewayOptions> paymentGatewayOptions,
    ILogger<ESewaPaymentGatewayService> logger,
    ApplicationDataContext context,
    HangfireJobHelper hangfireJobHelper, ICustomerPolicyCreateJobService customerProfileJobService) : IPaymentGatewayService
{
    private readonly ESewaConfig _eSewaConfig = paymentGatewayOptions?.Value?.Esewa
                        ?? throw new InvalidOperationException("ESewa configuration is missing for ESewaPaymentGatewayService.");

    public async Task<object> InitiatePaymentAsync(PolicyDraft purchase)
    {
        logger.LogInformation($"ESewaPaymentGatewayService: Initiating payment for purchase ID: {purchase.Id}");

        if (string.IsNullOrEmpty(purchase.TransactionReference))
        {
            logger.LogError("TransactionReference is missing on PolicyDraft for eSewa initiation.");
            throw new InvalidOperationException("PolicyDraft must have a TransactionReference to initiate eSewa payment.");
        }

        var eSewaInitiationData = new PaymentInitiationRequest
        {
            Amount = purchase.NetPremium, // Assuming PolicyDraft.Amount is the total_amount for eSewa
            TaxAmount = 0, // Placeholder, populate from PolicyDraft if applicable
            ServiceCharge = 0, // Placeholder
            DeliveryCharge = 0 // Placeholder
        };

        // Construct specific success and failure URLs.
        // These would typically be callback endpoints in your application.
        var successUrl = $"{_eSewaConfig.SuccessCallbackUrl}";
        var failureUrl = $"{_eSewaConfig.FailureCallbackUrl}";

        var formData = eSewaService.GenerateFormDataForPayment(
            eSewaInitiationData,
            purchase.TransactionReference,
            successUrl,
            failureUrl
        );

        logger.LogInformation($"ESewaPaymentGatewayService: Generated eSewa HTML form for purchase ID: {purchase.Id}.");

        return (formData);
    }

    public async Task<bool> VerifyPaymentAsync(VerifyPaymentRequestModel requestModel)
    {
        var payloadBytes = Convert.FromBase64String(requestModel.PayloadBase64);
        var payloadJson = Encoding.UTF8.GetString(payloadBytes);

        var parsedModel = JsonSerializer.Deserialize<ESewaCallbackResponse>(payloadJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        logger.LogInformation("ESewaPaymentGatewayService: Verifying payment for transaction reference: {TxnId}, Amount: {Amount}",
            parsedModel.TransactionUuid, parsedModel.TotalAmount);

        try
        {
            if (!eSewaService.VerifyCallbackSignature(parsedModel))
            {
                logger.LogWarning("Invalid signature for transaction {TxnId}", parsedModel.TransactionUuid);
                return false;
            }

            var isPaymentComplete = parsedModel.Status.Equals("COMPLETE", StringComparison.OrdinalIgnoreCase);

            var policyDraft = await context.PolicyDrafts
                .Include(x => x.PaymentTransaction)
                .FirstOrDefaultAsync(x => x.TransactionReference == parsedModel.TransactionUuid);

            if (policyDraft == null)
            {
                logger.LogWarning("Transaction not found for UUID: {TxnId}", parsedModel.TransactionUuid);
                return false;
            }
            var transaction = policyDraft.PaymentTransaction;

            transaction.RawResponse = payloadJson;
            transaction.PaymentGatewayStatus = parsedModel.Status;
            transaction.VerifiedAt = DateTime.UtcNow;
            transaction.Status = isPaymentComplete ? PaymentTransactionStatus.Verified : PaymentTransactionStatus.Failed;

            if (!isPaymentComplete)
            {
                logger.LogWarning("Payment for transaction {TxnId} failed with status: {Status}", parsedModel.TransactionUuid, parsedModel.Status);
                policyDraft.Status = PurchaseStatus.Failed;
                context.PolicyDrafts.Update(policyDraft);
                context.PaymentTransactions.Update(transaction);
                await context.SaveChangesAsync();
                return false;
            }

            if (isPaymentComplete)
            {
                policyDraft.Status = PurchaseStatus.Paid;
                hangfireJobHelper.EnqueueWithLogging<ICustomerPolicyCreateJobService>(
                 job => job.CreatePolicyAsync(policyDraft.InsuranceType.Value, transaction.Id, CancellationToken.None),
                 $"CreatePolicy job for {transaction.Id}"
             );

            }
            context.PolicyDrafts.Update(policyDraft);
            context.PaymentTransactions.Update(transaction);
            await context.SaveChangesAsync();
            logger.LogInformation("Payment for transaction {TxnId} verified successfully.", parsedModel.TransactionUuid);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying eSewa transaction {TxnId}: {Message}", parsedModel.TransactionUuid, ex.Message);
            throw;
        }
    }


}
