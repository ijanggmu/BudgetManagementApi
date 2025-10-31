using Models.Common.Policy.Policy;
using SharedKernel.SystemEnum.Payment;

namespace Models.WebApi.Policy.Motor;
public class MotorPolicyRequestModel
{
    public MotorPartialModel MotorPartial { get; set; }
    public string DraftNumber { get; set; }
    public string PortfolioAlias { get; set; }
    public string PortfolioId { get; set; }
    public string PartyId { get; set; }
    public string TypeOfParty { get; set; }
    public string PortfolioParent { get; set; }
    public string Class { get; set; }
    public decimal NetPremium { get; set; }
    public decimal PayablePremium { get; set; }
    public PaymentGateway Gateway { get; set; }
    public string UserId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ProposedDate { get; set; }

}
public class MotorPartialModel
{
    public bool IsThirdParty { get; set; }
    public bool IsComprehensive { get; set; }
    public string Type { get; set; }
    public string ManufactureYear { get; set; }
    public string ManufactureCompany { get; set; }
    public string Model { get; set; }
    public bool PurchasedNewOld { get; set; }
    public string DateOfPurchase { get; set; }
    public string ChasisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public decimal? CubicCapacity { get; set; }
    public decimal? KilloWatt { get; set; }
    public string Days { get; set; }
    public string YearsFromRegistrationDateYears { get; set; }
    public string YearsFromRegistrationDateYearsBS { get; set; }
    public string CurrentMarketPrice { get; set; }
    public int? AgeOfVehicle { get; set; }
    public bool RiotStrike { get; set; }
    public int VechileType { get; set; }
}
