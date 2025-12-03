using Data.Entities.PolicyE2e;
using Models.Common.PolicyPayment;

namespace Business.Common.PolicyPayment.PaymentGateway;
public interface IPaymentGatewayService
{
    /// <summary>
    /// Initiates a payment process with the specific gateway.
    /// The return string could be a redirect URL or an HTML form string for auto-submission.
    /// </summary>
    /// <param name="purchase">The policy draft or purchase object containing payment details.</param>
    /// <returns>A string representing a redirect URL or an auto-submitting HTML form.</returns>
    Task<object> InitiatePaymentAsync(PolicyDraft purchase);

    /// <summary>
    /// Verifies a payment transaction with the specific gateway.
    /// This typically involves a server-to-server API call to the gateway.
    /// </summary>
    /// <param name="transactionReference">Our system's unique transaction identifier.</param>
    /// <param name="amount">The total amount of the transaction to verify.</param>
    /// <returns>True if the payment is successfully verified, false otherwise.</returns>
    Task<bool> VerifyPaymentAsync(VerifyPaymentRequestModel requestModel);
}
