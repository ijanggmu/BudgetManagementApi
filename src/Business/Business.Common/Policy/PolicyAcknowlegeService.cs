using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common.Policy.ThirdPartyApi;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Payment;

namespace Business.Common.Policy;

public class PolicyAcknowlegeService(ApplicationDataContext context, ILogger<PolicyAcknowlegeService> logger) : IPolicyAcknowlegeService
{
    public async Task<Result<string>> AcknowledgeCreationAndStoreDetailsAsync(ThirdPartyPolicyCreateResponse responseModel)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            logger.LogInformation("Attempting to acknowledge policy creation for Draft ID: {DraftId}", responseModel.draftNo);

            var draft = await context.PolicyDrafts
                .Include(x => x.Customer)
                .Where(x => x.DraftNo == responseModel.draftNo)
                .FirstOrDefaultAsync();

            if (draft == null)
            {
                logger.LogWarning("No PolicyDraft found for Draft Number: {DraftId}", responseModel.draftNo);
                return Result<string>.Failed("PolicyDraft not found.");
            }

            if (draft.Customer == null)
            {
                logger.LogWarning("No Customer associated with PolicyDraft Number: {DraftId}", responseModel.draftNo);
                return Result<string>.Failed("Customer not found for the policy.");
            }

            draft.PortfolioAlias = responseModel.PortfolioAlias;
            draft.Customer.PartyCode = responseModel.PartyCode;
            draft.PolicyNumber = responseModel.policyNumber;
            draft.DocumentNumber = responseModel.documentNumber;
            draft.ReceiptNumber = responseModel.receiptNumber;
            draft.InvoiceNumber = responseModel.invoiceNumber;
            draft.Status = PurchaseStatus.Acknowledged;
            context.PolicyDrafts.Update(draft);
            context.Customers.Update(draft.Customer);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            logger.LogInformation("Successfully acknowledged policy for Draft Number: {DraftId}", responseModel.draftNo);
            return Result<string>.Success("Policy Acknowledge Successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Error occurred while acknowledging policy for Draft Number: {DraftId}", responseModel.draftNo);
            throw;
        }
    }
}
