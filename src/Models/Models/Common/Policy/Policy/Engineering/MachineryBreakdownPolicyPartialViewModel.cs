using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class MachineryBreakdownPolicyPartialViewModel : ILocation
    {
        public List<MBMaterialOfInsurance> MaterialsOfInsurance { get; set; }
        public List<EndorsedMBMaterialOfInsurance> AddedMaterialsOfInsurance { get; set; }
        public List<EndorsedMBMaterialOfInsurance> UpdatedMaterialsOfInsurance { get; set; }
        public List<EndorsedMBMaterialOfInsurance> DiscontinuedMaterialsOfInsurance { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsTPLSelected { get; set; }
        public decimal ExcessAmountForTypeA { get; set; }
        public decimal ExcessAmountForTypeB { get; set; }
        public decimal ExcessAmountForTypeC { get; set; }
        [Range(0.001,100,ErrorMessage = "Please enter a value greater than 0 and less than 100")]
        public decimal ExcessRateForTypeA { get; set; }
        [Range(0.001, 100, ErrorMessage = "Please enter a value greater than 0 and less than 100")]
        public decimal ExcessRateForTypeB { get; set; }
        [Range(0.001, 100, ErrorMessage = "Please enter a value greater than 0 and less than 100")]
        public decimal ExcessRateForTypeC { get; set; }
        public decimal? SumInsuredForTPL { get; set; }
        public string Premises { get; set; }
        //[Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        [Range(0.001, 100, ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal? BasicPremiumRate { get; set; }
        public decimal ExcessRate { get; set; }
        public decimal ExcessAmount { get; set; }
        public List<string> MachineryBreakdownEndorsementList { get; set; }
        public string DeductiblesText { get; set; }
        public string[] Locations { get; set; }

        public string Name { get; set; }
        public decimal? Megawatt { get; set; }
        public string Province { get; set; }
       
        public string District { get; set; }
       
        public string Municipality { get; set; }
       
        public string Ward { get; set; }
       
        public string StreetAddress { get; set; }
    }

    public class MBMaterialOfInsurance
    {
        public string ItemNo { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string MakersName { get; set; }
        public string CountryOfOrigin { get; set; }
        public int YearOfMake { get; set; }
        public decimal ReplacementValue { get; set; }
        public decimal FullSumInsured { get; set; } 
        public string Identifier { get; set; }
    }

    public class EndorsedMBMaterialOfInsurance : MBMaterialOfInsurance
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}
