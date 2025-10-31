using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.WebApi.Customer;

namespace Models.WebApi.Policy.Motor;
public class CreateMotorPolicyRequestModel
{
    public string DraftNo { get; set; }
    public string CustomerEmail { get; set; }
    public string GateWay { get; set; }
    public string TransactionId { get; set; }
    public MotorRequest Motor { get; set; }
    public CustomerRequest Customer { get; set; }
   
    public string PortfolioAlias { get; set; }
    public string PortfolioId { get; set; }
    public string PartyId { get; set; }
    public string TypeOfParty { get; set; }
    public string PortfolioParent { get; set; }
    public string Class { get; set; }
    public string UserId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ProposedDate { get; set; }
}

public class CustomerRequest
{
    public string FullName { get; set; }
    public string UserId { get; set; }
    public string IndividualType { get; set; }
    public string CourtesyTitle { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullNameNepali { get; set; }
    public string Gender { get; set; }
    public string PanNo { get; set; }
    public string MaritialStatus { get; set; }
    public string PaProvince { get; set; }
    public string PaDistrict { get; set; }
    public string PaMunicipality { get; set; }
    public string PaWard { get; set; }
    public string PaStreetAddress { get; set; }
    public string TmProvince { get; set; }
    public string TmDistrict { get; set; }
    public string TmMunicipality { get; set; }
    public string TmWard { get; set; }
    public string TmStreetAddress { get; set; }
    public DateTime DobBS { get; set; }
    public DateTime DobAD { get; set; }
    public string CitizenshipNo { get; set; }
    public string CitizenshipIssueDistrict { get; set; }
    public string CitizenshipIssueDate { get; set; }
    public string PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public string PassportExpiryDate { get; set; }
    public string PassportIssuePlace { get; set; }
    public string VoterIdNumber { get; set; }
    public string LicenseNumber { get; set; }
    public List<string> OccupationJson { get; set; }
}

public class MotorRequest
{
    public bool IsThirdParty { get; set; }
    public bool IsComprehensive { get; set; }
    public string Type { get; set; }
    public string ManufactureYear { get; set; }
    public string ManufactureCompany { get; set; }
    public string Model { get; set; }
    public bool PurchasedNewOld { get; set; }
    public DateTime DateOfPurchase { get; set; }
    public string ChasisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public string CubicCapacity { get; set; }
    public int Days { get; set; }
    public string YearsFromRegistrationDateYears { get; set; }
    public string YearsFromRegistrationDateYearsBS { get; set; }
    public decimal CurrentMarketPrice { get; set; }
    public int? AgeOfVehicle { get; set; }
    public bool RiotStrike { get; set; }
}
