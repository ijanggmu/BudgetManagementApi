using Models.WebApi.ITI;
using Models.WebApi.Policy.Motor;
using SharedKernel.SystemEnum.Payment;

namespace Models.WebApi.Customer.Policy;

public class PolicyDraftByIdResponseModel
{
    public string DraftNo { get; set; }
    public string StaffEmail { get; set; }
    public PaymentGateway? GateWay { get; set; }
    public string TransactionId { get; set; }
    public decimal NetPremium { get; set; }
    public DateTime CreatedOn { get; set; }
    public string PolicyId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public CustomerResponseModel Customer { get; set; }
    public MotorResponseModel Motor { get; set; }
    public ITIResponseModel ITI { get; set; }

}
