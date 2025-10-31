using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class ConstructionPlantAndMachineryPolicyPartialViewModel
    {
        public List<CPMMaterialOfInsurance> MaterialsOfInsurance { get; set; }
        public List<EndorsedCPMMaterialOfInsurance> AddedMaterialsOfInsurance { get; set; }
        public List<EndorsedCPMMaterialOfInsurance> UpdatedMaterialsOfInsurance { get; set; }
        public List<EndorsedCPMMaterialOfInsurance> DiscontinuedMaterialsOfInsurance { get; set; }

        public decimal ExcavatorMinimumExcessAmount { get; set; }
        public decimal ExcavatorMinimumExcessRate { get; set; }
        public decimal LoaderMinimumExcessAmount { get; set; }
        public decimal LoaderMinimumExcessRate { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsTPLSelected { get; set; }
        public decimal? SumInsuredForTPL { get; set; }
        //[Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public string Province { get; set; }
       
        public string District { get; set; }
       
        public string Municipality { get; set; }
       
        public string Ward { get; set; }
       
        public string StreetAddress { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        [Range(0.001, 100, ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal? BasicPremiumRate { get; set; }
        public string ConfirmationDetails { get; set; }
        public string Exclusions { get; set; }
        public string Deductibles { get; set; }
        public string AOG { get; set; }
        public string DeductibleType { get; set; }
        public decimal? BodilyInjured { get; set; }
        public decimal? PropertyDamage { get; set; }
        public string UseOfPlantAndMachinery { get; set; }
        public decimal? Megawatt { get; set; }
    }
    public class CPMMaterialOfInsurance
    {
        public string SN { get; set; }
        public string RegistrationNumber { get; set; }
        public string EngineNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ChassisNo { get; set; }
        public string Make { get; set; }
        public decimal YOM { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string CC { get; set; }
        public decimal SumInsured { get; set; }
        public decimal FullSumInsured { get; set; }
        public string Identifier { get; set; }
    }

    public class EndorsedCPMMaterialOfInsurance : CPMMaterialOfInsurance
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}
