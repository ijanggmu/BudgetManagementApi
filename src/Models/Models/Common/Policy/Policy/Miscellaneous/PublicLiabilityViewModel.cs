using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class PublicLiabilityViewModel
    {
        public List<PublicLiabilitiesViewModel> Properties { get; set; }
        public List<PublicLiabilityParaglidingViewModel> ParaglidingProperties { get; set; }
        public List<PublicLiabilityParaglidingViewModel> AddedParaglidingProperties { get; set; }
        public List<PublicLiabilityParaglidingViewModel> UpdatedParaglidingProperties { get; set; }
        public List<PublicLiabilityParaglidingViewModel> DeletedParaglidingProperties { get; set; }
        public decimal LimitLiablity { get; set; }
        public decimal FullLimitLiablity { get; set; }
        public decimal BasicRate { get; set; }
        public string Premises { get; set; }
        public string GeographicalExceptions { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool IsParagliding { get; set; }
        public bool IsClinicalTrial { get; set; }
        public bool IsAggregateLimit { get; set; }
        public decimal AggregateLimitofLiability { get; set; }
        public decimal FullAggregateLimitofLiability { get; set; }
        public decimal OldAggregateLimitofLiability { get; set; }
        public decimal NumberofGliderLimitOfPassenger { get; set; }
        public decimal PerGliderSILimitOfPassenger { get; set; }
        public decimal SILimitOfPassenger { get; set; }
        public decimal RateLimitOfPassenger { get; set; }
        public decimal PerGliderPremiumLimitOfPassenger { get; set; }
        public decimal NumberofGliderTPL { get; set; }
        public decimal PerGliderSITPL { get; set; }
        public decimal SITPL { get; set; }
        public decimal RateTPL { get; set; }
        public decimal PerGliderPremiumTPL { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal FullTotalPremium { get; set; }
        public decimal TotalPremiumLimitOFPassenger { get; set; }
        public decimal TotalPremiumTPL { get; set; }
        public decimal TotalSuminsured { get; set; }
        public decimal BasicPremium { get; set; }
        public List<string> PublicLiabilityEndorsements { get; set; }
        public string Business { get; set; }
        public bool IsPremiumManual { get; set; }
        public string ProtocolNumber { get; set; }
        public decimal ManualSuminsured { get; set; }
        public decimal FullManualSuminsured { get; set; }
        public decimal OldManualSuminsured { get; set; }
        public string StudyTitle { get; set; }
        public decimal? DeductibleRate { get; set; }
        public decimal? DeductibleAmount { get; set; }
        public string PolicyTerritory { get; set; }
        public string Trigger { get; set; }
        public string LimitOfLiability { get; set; }
        public decimal ChangeInCountNumberofGliderLimitOfPassenger { get; set; }
        public decimal ChangeInCountNumberofGliderTPL { get; set; }
        public string Note { get; set; }
        public string EstimatedPatients { get; set; }
        public string Exception { get; set; }
        public string PLTrigger { get; set; }
        public decimal? PlDeductibleRate { get; set; }
        public decimal? PlDeductibleAmount { get; set; }
        public string DeductiblesText { get; set; }
    }

    public class PublicLiabilitiesViewModel
    {
        public string SN { get; set; }
        public decimal AnyOneAccidentA { get; set; }
        public decimal AnyOnePerson { get; set; }
        public decimal AnyOneYearA { get; set; }
        public decimal FullAnyOneYearA { get; set; }
        public decimal AnyOneAccidentB { get; set; }
        public decimal AnyOneYearB { get; set; }
        public decimal FullAnyOneYearB { get; set; }
    }
    public class PublicLiabilityParaglidingViewModel
    {
        public string PrimaryId { get; set; }
        public string SN { get; set; }
        public string NameOfPilot { get; set; }
        public string GliderType { get; set; }
        public string SerialNumber { get; set; }
        public string DOM { get; set; }
        public string FlightType { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
    }
  
}
