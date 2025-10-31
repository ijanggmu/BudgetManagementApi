using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class TIBPPartialViewModel
    {
        public decimal USDPerDayRate { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal CovidLoadingCharge { get; set; }
        public string TravellingCountry { get; set; }
        public bool ChooseCoronaVirus { get; set; }
        public string Plan { get; set; }
        public string Cover { get; set; }
        public string Benefits { get; set; }
        public string GeographicalAreas { get; set; }
        public string  MaxDays { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public int NumberOfDays { get; set; } // Total days 
        public int CalculatedDays { get; set; } // User entered days
        public int CarryOverDays { get; set; } // Remaining or unused days 
        public int TotalNumberOfDaysRemaining { get; set; }
        public int EndorsedNumberOfDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public decimal BasicPremiumForCancellation { get; set; }
        public string PersonalAccidentSI { get; set; }
        public string  PersonalAccidentExcess { get; set; }
        public string EmergencyMedicalExpensesSI { get; set; }
        public string  EmergencyMedicalExpensesExcess { get; set; }
        public string EmergencyDentalCareSI { get; set; }
        public string  EmergencyDentalCareExcess { get; set; }
        public string RepatriationSI { get; set; }
        public string  RepatriationExcess { get; set; }
        public string TravelSI { get; set; }
        public string  TravelExcess { get; set; }
        public string HospitalBenefitsSI { get; set; }
        public string  HospitalBenefitsExcess { get; set; }
    }
}
