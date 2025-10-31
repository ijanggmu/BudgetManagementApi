using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Aviation
{
    public class AviationLOLPartialViewModel
    {
        
        public decimal TotalSumInsured { get; set; }
        public string  Benefits { get; set; }
        public string MaximumBenefitPeriod { get; set; }
        public string WaitingPeriod { get; set; }
        public string DepositPremium { get; set; }
        [Required(ErrorMessage = ("Please select a party"))]
        public decimal AnnualPremium { get; set; }
        public decimal Premium { get; set; }
        public int Days { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<AviationInsuredPersionViewModel> AviationInsuredPersons { get; set; }

        public List<AircraftPremiumInstalmentViewModel> PremiumInstalment { get; set; }
        public string InvoiceText { get; set; }
        public decimal CurrentUSDExchangeRate { get; set; }
        public List<EndorseAviationInsuredPersionViewModel> AddedAviationInsuredPersons { get; set; }
        public List<EndorseAviationInsuredPersionViewModel> UpdatedAviationInsuredPersons { get; set; }
        public List<EndorseAviationInsuredPersionViewModel> DiscontinuedAviationInsuredPersons { get; set; }
    }
}
