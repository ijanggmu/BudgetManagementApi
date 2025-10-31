using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Aviation
{
    public class AviationHallwarPartialViewModel
    {
        public string GeographicalLimits { get; set; }
        public string Limits { get; set; }
        public List<AircraftListViewModel> AircraftInformation { get; set; }
        public string ApprovedLineHolders { get; set; }
        public string Excluding { get; set; }
        public decimal Premium { get; set; }
        public decimal TotalSumInsured { get; set; }
        [Required(ErrorMessage = ("Please select a party"))]
        public decimal AnnualPremium { get; set; }
        public List<AircraftPremiumInstalmentViewModel> PremiumInstalment { get; set; }
        public decimal CurrentUSDExchangeRate { get; set; }
        public string Interest { get; set; }
        public string SumInsuredHull { get; set; }
        public string SumInsuredAircraftSpares { get; set; }
        public string SumInsuredExtortion { get; set; }
        public string ExtortionandHighjack { get; set; }
        public string SumInsuredConfiscation { get; set; }
        public int Days { get; set; }
        public string Situtaions { get; set; }
        public string PaymentTerms { get; set; }
        public string Notes { get; set; }
        public string InvoiceText { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<EndorseAircraftListViewModel> AddedAircraftInformation { get; set; }
        public List<EndorseAircraftListViewModel> UpdatedAircraftInformation { get; set; }
        public List<EndorseAircraftListViewModel> DiscontinuedAircraftInformation { get; set; }
    }
}
