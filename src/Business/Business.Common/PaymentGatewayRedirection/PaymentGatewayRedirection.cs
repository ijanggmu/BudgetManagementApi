using Business.Common.JobHelper.CustomerPolicyJob;
using Business.Common.JobHelper;
using System.Text.Json;
using System.Text;
using Business.Common.PolicyPayment.PaymentGateway;
using Business.Common.PolicyPayment.PaymentGateway.Esewa;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Common.PolicyPayment;
using SharedKernel.Config;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Payment;

namespace Business.Common.PaymentGatewayRedirection;
public interface IPaymentGatewayRedirectionService
{
    Task<Result<VerifyEsewaPaymentResponseModel>> EsewaSuccessAsync(string data);
    Task<Result<VerifyEsewaPaymentResponseModel>> EsewaFailureAsync(string data);
    Task<Result<VerifyEsewaPaymentResponseModel>> KhaltiStatusAsync(KhaltiPaymentRedirectResponseModel requestModel);
}
public class PaymentGatewayRedirectionService(
    IOptions<FrontEndUrlOptions> frontendUrl,
    ESewaService eSewaService,
   Dictionary<string, IPaymentGatewayService> paymentGateways, ApplicationDataContext context,
   ILogger<PaymentGatewayRedirectionService> logger, HangfireJobHelper hangfireJobHelper) : IPaymentGatewayRedirectionService
{
    public async Task<Result<VerifyEsewaPaymentResponseModel>> EsewaSuccessAsync(string data)
    {

        if (!paymentGateways.TryGetValue(nameof(PaymentGateway.Esewa), out var gateway))
            return Result<VerifyEsewaPaymentResponseModel>.Failed($"Unsupported payment gateway: {PaymentGateway.Esewa}");

        var isPaymentVerified = await gateway.VerifyPaymentAsync(new VerifyPaymentRequestModel()
        {
            PayloadBase64 = data,
            PaymentGateway = PaymentGateway.Esewa
        });

        if (!isPaymentVerified)
            return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-failure"));

        return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-success"));
    }
    public async Task<Result<VerifyEsewaPaymentResponseModel>> EsewaFailureAsync(string data)
    {
        if (string.IsNullOrEmpty(data))
            return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-failure"));

        if (!paymentGateways.TryGetValue(nameof(PaymentGateway.Esewa), out var gateway))
            return Result<VerifyEsewaPaymentResponseModel>.Failed($"Unsupported payment gateway: {PaymentGateway.Esewa}");

        var updatePaymentStatus = await gateway.VerifyPaymentAsync(new VerifyPaymentRequestModel()
        {
            PayloadBase64 = data,
            PaymentGateway = PaymentGateway.Esewa
        });

        return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-failure"));
    }

    public async Task<Result<VerifyEsewaPaymentResponseModel>> KhaltiStatusAsync(KhaltiPaymentRedirectResponseModel requestModel)
    {
        try
        {

            var isPaymentComplete = requestModel.Status.Equals("COMPLETE", StringComparison.OrdinalIgnoreCase);

            var policyDraft = await context.PolicyDrafts
            .Include(x => x.PaymentTransaction)
                .FirstOrDefaultAsync(x => x.TransactionReference == requestModel.TransactionId);

            if (policyDraft == null)
            {
                logger.LogWarning("Transaction not found for UUID: {TxnId}", requestModel.TransactionId);
                return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-failure?trancstionId={requestModel.TransactionId}"));
            }
            var transaction = policyDraft.PaymentTransaction;
            var serializedResponse = JsonSerializer.Serialize(requestModel);
            transaction.RawResponse = serializedResponse;
            transaction.PaymentGatewayStatus = requestModel.Status;
            transaction.VerifiedAt = DateTime.UtcNow;
            transaction.Status = isPaymentComplete ? PaymentTransactionStatus.Verified : PaymentTransactionStatus.Failed;

            if (!isPaymentComplete)
            {
                logger.LogWarning("Payment for transaction {TxnId} failed with status: {Status}", requestModel.TransactionId, requestModel.Status);
                policyDraft.Status = PurchaseStatus.Failed;

                context.PolicyDrafts.Update(policyDraft);
                context.PaymentTransactions.Update(transaction);
                await context.SaveChangesAsync();
                return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-success?trancstionId={requestModel.TransactionId}"));
            }

            if (isPaymentComplete)
            {
                hangfireJobHelper.EnqueueWithLogging<ICustomerPolicyCreateJobService>(
                     job => job.CreatePolicyAsync(Models.Common.Policy.Policy.InsuranceType.ThirdPartyBike, transaction.Id, CancellationToken.None),
                     $"CreatePolicy job for {transaction.Id}"
                 );
                policyDraft.Status = PurchaseStatus.Paid;

                context.PolicyDrafts.Update(policyDraft);
                context.PaymentTransactions.Update(transaction);
                await context.SaveChangesAsync();
                return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-success?trancstionId={requestModel.TransactionId}"));
            }

            logger.LogInformation("Payment for transaction {TxnId} verified successfully.", requestModel.TransactionId);
            return Result<VerifyEsewaPaymentResponseModel>.Success(new VerifyEsewaPaymentResponseModel($"{frontendUrl.Value.CustomerPortal}/payment-failure?trancstionId={requestModel.TransactionId}"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying eSewa transaction {TxnId}: {Message}", requestModel.TransactionId, ex.Message);
            throw;
        }
    }
}
