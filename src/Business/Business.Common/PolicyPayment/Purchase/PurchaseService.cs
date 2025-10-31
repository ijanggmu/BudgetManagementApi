using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;
using Business.Common.PolicyPayment.PaymentGateway;
using Data.Context;
using Data.Entities.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common.PolicyPayment;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Payment;

namespace Business.Common.PolicyPayment.Purchase;
public class PurchaseService(ApplicationDataContext dataContext, Dictionary<string, IPaymentGatewayService> gateways) : IPurchaseService
{

    public async Task<Result<object>> InitiatePaymentAsync(InitiatePaymentRequest requestModel)
    {
        await using var transaction = await dataContext.Database.BeginTransactionAsync();
        try
        {
            var policy = await dataContext.PolicyDrafts.FindAsync(requestModel.PolicyId);
            if (policy == null)
                return Result<Dictionary<string, string>>.Failed("Policy not found.");

            if (policy.Status != PurchaseStatus.Draft)
                return Result<Dictionary<string, string>>.Failed("Only draft purchases can be initiated");

            policy.PaymentGateway = requestModel.PaymentGateway;
            policy.TransactionReference = Guid.NewGuid().ToString("N");
            policy.Status = PurchaseStatus.Draft;


            if (!gateways.TryGetValue(requestModel.PaymentGateway.ToString(), out var gateway))
                return Result<Dictionary<string, string>>.Failed($"Unsupported payment gateway: {requestModel.PaymentGateway}");
            var response = await gateway.InitiatePaymentAsync(policy);

            var paymentTransaction = new PaymentTransaction()
            {
                PolicyPurchaseId = policy.Id,
                Amount = policy.NetPremium,
                RequestedAt = DateTime.UtcNow,
                PaymentGateway = requestModel.PaymentGateway
            };

            await dataContext.PaymentTransactions.AddAsync(paymentTransaction);
            await dataContext.SaveChangesAsync();

            policy.PaymentTransactionId = paymentTransaction.Id;

            dataContext.PolicyDrafts.Update(policy);
            await dataContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return Result<object>.Success(response);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> VerifyAsync(VerifyPaymentRequestModel requestModel)
    {
        if (!gateways.TryGetValue(requestModel.PaymentGateway.ToString(), out var gateway))
            return Result<bool>.Failed($"Unsupported payment gateway: {requestModel.PaymentGateway}");

        var verified = await gateway.VerifyPaymentAsync(requestModel);

        await dataContext.SaveChangesAsync();
        return verified;

    }

}





