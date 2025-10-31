using SharedKernel.SystemEnum.Payment;

public class PaymentInitiationRequest
{
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; } = 0; // Default to 0 as per docs
    public decimal ServiceCharge { get; set; } = 0; // Default to 0 as per docs
    public decimal DeliveryCharge { get; set; } = 0; // Default to 0 as per docs
    public string PolicyName { get; set; } // For your internal use
    public string CustomerEmail { get; set; } // For your internal use
    public string CustomerMobile { get; set; } // For your internal use
}

