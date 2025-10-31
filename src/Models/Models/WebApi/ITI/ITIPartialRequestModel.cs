using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Policy.Miscellaneous;
using SharedKernel.SystemEnum.Payment;

namespace Models.WebApi.ITI;
public class ITIPartialRequestModel
{
    public int PolicyPeriodInDays { get; set; }
    public string TripType { get; set; }
    public string IdType { get; set; }
    public string PassportNumber { get; set; }
    public string VisitingCountry { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactNumber { get; set; }
    public string TravellingCountry { get; set; }
    public string InsuranceType { get; set; }
    public string TypeOfInsured { get; set; }
    public string DraftNumber { get; set; }
    public decimal NetPremium { get; set; }
    public decimal PayablePremium { get; set; }
  
    public string Phone { get; set; }
    public PaymentGateway Gateway { get; set; }
    public List<FamilyMembersITI> FamilyMembers { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ProposedDate { get; set; }
    public DateTime EffectiveDateBeforeEndorsement { get; set; }
    public DateTime ExpiryDateBeforeEndorsement { get; set; }
}
public class FamilyMembersITI
{
    public string Relation { get; set; }
    public string FullName { get; set; }
    public string PassportNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
}
