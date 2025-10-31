using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class MotorcycleCalculationsViewModel
    {
        public int AgeOfVehicle { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal AgeLoadingAmount { get; set; }
        public decimal VoluntaryExcessAmount { get; set; }
        public decimal NoClaimDiscountAmount { get; set; }
        public decimal SubTotalA { get; set; }
        public decimal ThirdPartyAmount { get; set; }
        public decimal ThirdPartyNoClaimDiscountAmount { get; set; }
        public decimal SubTotalB { get; set; }
        public decimal RiotAndStrikeAndMdAmount { get; set; }
        public decimal TerrorismAmount { get; set; }
        public decimal PayToRiderAndOnePillionRiderAmount { get; set; }
        public decimal SubTotalC { get; set; }
        public decimal StampDutyAmount { get; set; }
        public decimal VatPercent { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalPremiumAmount { get; set; }
        public decimal NetPremiumAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string ManufactureCompany { get; set; }
        public string EngineNumber { get; set; }
        public string ChasisNumber { get; set; }
        public decimal CubicCapacity { get; set; }
        public string RegistrationNumber { get; set; }
        public string Model { get; set; }
    }
}
