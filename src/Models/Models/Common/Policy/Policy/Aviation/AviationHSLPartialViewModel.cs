using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Aviation
{
    public class AviationHSLPartialViewModel
    {
        public string RiskCovered { get; set; }
        public string StandardUses { get; set; }
        public string SpecialUses { get; set; }
        public string SpecialRentalUses { get; set; }
        public string Pilots { get; set; }
        public string GeographicalLimits { get; set; }
        public string Type { get; set; }
        public int InstalmentNumber { get; set; }
        public int Days { get; set; }

        public List<AircraftListViewModel> AircraftInformation { get; set; }

        public string LossOfDamageAmt { get; set; }
        public string LossOfDamageLimit { get; set; }

        public string LiabilityToTplAmt { get; set; }
        public string LiabilityToTplLimit { get; set; }

        public string LiabilityToPassengersAmt { get; set; }
        public string LiabilityToPassengerLimit { get; set; }

        public string LiabilityToCombinedAmt { get; set; }
        public string LiabilityToCombinedLimit { get; set; }

        public string LiabilityToCargoAmt { get; set; }
        public string LiabilityToCargoLimit { get; set; }

        public string LiabilityToCrewAmt { get; set; }
        public string LiabilityToCrewLimit { get; set; }

        public string LiabilityToAircraftSprAmt { get; set; }
        public string LiabilityToAircraftSprLimit { get; set; }

        public string SupplementaryPaymentAmount { get; set; }
        public string SupplementaryPaymentLimit { get; set; }

        public string Interest { get; set; }


        public string SumInsuredHull { get; set; }
        public string SumInsuredAircraftSpares { get; set; }
        public string SumInsuredPersonalAccident { get; set; }
        public string SumInsuredPremisesAndProduct { get; set; }

        public string Deductibles { get; set; }

        public decimal Premium { get; set; }
        public decimal TotalSumInsured { get; set; }
        [Required(ErrorMessage = ("Please select a party"))]
        public decimal AnnualPremium { get; set; }
        public string PolicyClause { get; set; }
        public List<AircraftPremiumInstalmentViewModel> PremiumInstalment { get; set; }
        public decimal CurrentUSDExchangeRate { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Notes { get; set; }
        public string InvoiceText { get; set; }

        public List<EndorseAircraftListViewModel> AddedAircraftInformation { get; set; }
        public List<EndorseAircraftListViewModel> UpdatedAircraftInformation { get; set; }
        public List<EndorseAircraftListViewModel> DiscontinuedAircraftInformation { get; set; }
    }
}
