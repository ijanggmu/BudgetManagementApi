using Models.WebApi.Customer.Policy;

namespace Models.WebApi.ITI;
public class ITIResponseModel
{
    public int PolicyPeriodInDays { get; set; }
    public TripType TripType { get; set; }
    public string PassportNumber { get; set; }
    public string VisitingCountryString { get; set; }

    public List<string> VisitingCountry { get; set; }
    public string FatherOrHusbandName { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactNumber { get; set; }
    public TravelInsuranceType TravelInsuranceType { get; set; }
    public string TypeOfInsured { get; set; }
    public TravelPlanType PlanType { get; set; }
    public bool IsFamilyIncluded { get; set; }
    public string TravellingCountry { get; set; }
    public List<ITIFamilyMemberResponseModel> FamilyMembers { get; set; }
}
