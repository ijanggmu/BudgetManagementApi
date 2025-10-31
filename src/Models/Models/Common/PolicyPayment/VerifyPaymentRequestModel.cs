using SharedKernel.SystemEnum.Payment;

namespace Models.Common.PolicyPayment;

public class VerifyPaymentRequestModel
{
    public PaymentGateway PaymentGateway { get; set; }
    public string PayloadBase64 { get; set; }
}
