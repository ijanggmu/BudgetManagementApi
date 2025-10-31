namespace Models.Common.PolicyPayment;

public class PaymentVerifyRequest
{
    public string TransactionReference { get; set; } = default!;
    public decimal Amount { get; set; }
}
