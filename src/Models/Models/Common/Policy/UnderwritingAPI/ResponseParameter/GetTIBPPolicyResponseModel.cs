using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
   public class GetTIBPPolicyResponseModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string PANNumber { get; set; }
        
        public DateTime IssueDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string FiscalYear { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public string BranchName { get; set; }
        public string CustomerIdNumber { get; set; }
        public string DOAgentNumber { get; set; }
        public string BillNumber { get; set; }
        public string RctDateTime { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal Stampduty { get; set; }
        public decimal VatAmount { get; set; }
        public decimal NetPremium { get; set; }

        public string PartyName { get; set; }
        public string PartyCode { get; set; }
        public string PartyPanNumber { get; set; }
        public string Address { get; set; }
        public TIBPDetails tIBPDetails { get; set; }
    }
    public class TIBPDetails
    {
        public decimal USDPerDayRate { get; set; }
        public decimal ExchangeRate { get; set; }
        public string TravellingCountry { get; set; }
        public decimal CovidLoadingCharge { get; set; }
        public bool ChooseCoronaVirus { get; set; }
        public string Plan { get; set; }
        public string Benefits { get; set; }
        public string Cover { get; set; }
        public string GeographicalAreas { get; set; }
        public string MaxDays { get; set; }
        public int NumberOfDays { get; set; }
        public int EndorsedNumberOfDays { get; set; }
        public string PolicyType { get; set; }
        public string EndorsementType { get; set; }
        public string PersonalAccidentSI { get; set; }
        public string PersonalAccidentExcess { get; set; }
        public string EmergencyMedicalExpensesSI { get; set; }
        public string EmergencyMedicalExpensesExcess { get; set; }
        public string EmergencyDentalCareSI { get; set; }
        public string EmergencyDentalCareExcess { get; set; }
        public string RepatriationSI { get; set; }
        public string RepatriationExcess { get; set; }
        public string TravelSI { get; set; }
        public string TravelExcess { get; set; }
        public string HospitalBenefitsSI { get; set; }
        public string HospitalBenefitsExcess { get; set; }

    }

}
