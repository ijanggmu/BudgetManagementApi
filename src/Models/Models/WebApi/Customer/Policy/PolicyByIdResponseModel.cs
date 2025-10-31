using Models.WebApi.Address;
using Models.WebApi.ITI;
using Models.WebApi.Policy.Motor;
using SharedKernel.SystemEnum.Payment;

namespace Models.WebApi.Customer.Policy;
public class PolicyByIdResponseModel
{
    public string DraftNo { get; set; }
    public string StaffEmail { get; set; }
    public PaymentGateway? GateWay { get; set; }
    public string TransactionId { get; set; }
    public decimal NetPremium { get; set; }
    public DateTime CreatedOn { get; set; }
    public string PolicyId { get; set; }
    public string Class { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public CustomerResponseModel Customer { get; set; }
    public MotorResponseModel Motor { get; set; }
    public ITIResponseModel ITI { get; set; }
}
public class GetAllDraftResponseModel
{
    public string DraftNo { get; set; }
    public decimal NetPremium { get; set; }
    public string PortfolioAlias { get; set; }
    public string Class { get; set; }
    public string Id { get; set; }
    public string InsuredName { get; set; }
    public string Model { get; set; }
    public string VistingCountry { get; set; }
    public string PolicyStatus { get; set; }
    public string CreatedOn { get; set; }
    public string ManufacturedDate { get; set; }
    public string ExpiryDate { get; set; }
    public string EffectiveDate { get; set; }
    public string VehicleType { get; set; }
    public string BlueBookCopyImageUrl { get; set; }
}
public class ITIFamilyMemberResponseModel
{
    public string Relation { get; set; }
    public string FullName { get; set; }
    public string PassportNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
}
public class GetAllActivePoliciesResponseModel
{
    public string Id { get; set; }
    public string DraftNo { get; set; }
    public decimal NetPremium { get; set; }
    public string PortfolioAlias { get; set; }
    public string Class { get; set; }
    public string InsuredName { get; set; }
    public string Model { get; set; }
    public string VistingCountry { get; set; }
    public string PolicyStatus { get; set; }
    public int RemainingDays { get; set; }
    public string PolicyNumber { get; set; }
    public string DocumentNumber { get; set; }
    public string CreatedOn { get; set; }
    public string ExpiryDate { get; set; }
    public string EffectiveDate { get; set; }
    public bool IsExpired { get; set; }

}
