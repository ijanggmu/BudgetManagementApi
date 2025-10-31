using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Customer.Policy;
public class InternationalTravelInsuranceRequestModel
{
    public int PolicyPeriodInDays { get; set; }
    public TripType TripType { get; set; }
    public string PassportNumber { get; set; }
    public List<string> VisitingCountry { get; set; }
    public string FatherOrHusbandName { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactNumber { get; set; }
    public TravelInsuranceType TravelInsuranceType { get; set; }
    public string TypeOfInsured { get; set; }
    public TravelPlanType PlanType { get; set; }
    public bool IsFamilyIncluded { get; set; }
    public List<ITIFamilyMemberRequestModel> FamilyMembers { get; set; }
}
public enum TravelPlanType
{
    A,
    B
}
public enum TripType
{
    Single = 1,
    Annual = 2,
    Return = 3
}
public enum TravelInsuranceType
{
    Individual,
    Family
}
public class ITIFamilyMemberRequestModel
{
    public string Relation { get; set; }
    public string FullName { get; set; }
    public string PassportNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
}

public class ITIFamilyMemberCalculatorRequestModel
{
    public int Age { get; set; }
}

public class InternationalTravelInsuranceCoreRequestModel
{
    public int PolicyPeriodInDays { get; set; }

    public string TripType { get; set; }

    public string PassportNumber { get; set; }

    public string VisitingCountry { get; set; }

    public string FatherHusbandName { get; set; }

    public string Gender { get; set; }

    public string Occupation { get; set; }


    public DateTime DateOfBirth { get; set; }

    public int Age { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string EmergencyContactName { get; set; }

    public string EmergencyContactNumber { get; set; }

    public string TravellingCountry { get; set; }

    public string InsuranceType { get; set; }

    public string TypeOfInsured { get; set; }

    public string Province { get; set; }

    public string District { get; set; }

    public string Municipality { get; set; }

    public string Ward { get; set; }
    public string PlanType { get; set; }

    public string StreetAddress { get; set; }

    public decimal PremiumAmount { get; set; }

    public decimal ExchangeRate { get; set; }
    public List<ITIFamilyMemberRequestModel> FamilyMembers { get; set; }
}

public class InternationalTravelInsurancePremiumCalculatorRequestModel
{
    [Required(ErrorMessage = "Policy period is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Policy period in days must be greater than 0.")]
    public int PolicyPeriodInDays { get; set; }

    [Required(ErrorMessage = "Trip type is required.")]
    [EnumDataType(typeof(TripType), ErrorMessage = "Invalid trip type.")]
    public TripType TripType { get; set; }

    [Required(ErrorMessage = "At least one visiting country must be provided.")]
    [MinLength(1, ErrorMessage = "At least one visiting country must be specified.")]
    public List<string> VisitingCountry { get; set; }

    [Required(ErrorMessage = "Age is required.")]
    [Range(19, 100, ErrorMessage = "Age must be greater than 18.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Type of insured is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Type of insured must be between 2 and 50 characters.")]
    public string TypeOfInsured { get; set; }

    [Required(ErrorMessage = "Travel plan type is required.")]
    [EnumDataType(typeof(TravelPlanType), ErrorMessage = "Invalid travel plan type.")]
    public TravelPlanType PlanType { get; set; }

    public bool IsFamilyIncluded { get; set; }
}


