using Models.Common.Policy;
using Models.Common.Policy.Policy;
using Models.WebApi.Customer.Policy;

namespace Models.BeemaEdgeApi.Customer.Policy;
public class SaveDraftRequestModel
{
    public InsuranceType InsuranceType { get; set; }
    public string PortfolioAlias { get; set; }
    public string TypeOfParty { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ProposedDate { get; set; }
    public string BancassuanceBankName { get; set; }
    public string BancassuanceBankBranch { get; set; }
    public decimal NetPremium { get; set; }
    public MotorRequestModel Motor { get; set; }
    public InternationalTravelInsuranceRequestModel InternationTravelInsurance { get; set; }
    public PrivateVehiclePartialViewModel PrivateVechile { get; set; }
}

