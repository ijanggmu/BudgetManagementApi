using SharedKernel.SystemEnum.Payment;

namespace Models.Common.PolicyPayment;
public class InitiatePaymentRequest
{
    public string PolicyId { get; set; }
    public PaymentGateway PaymentGateway { get; set; }
}
