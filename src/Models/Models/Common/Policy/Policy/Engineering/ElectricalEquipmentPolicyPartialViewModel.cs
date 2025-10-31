using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class ElectricalEquipmentPolicyPartialViewModel : ILocation
    {
        public List<MaterialOfInsurance> MaterialsOfInsurance { get; set; }
        public List<ExcessData> ExcessData { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        public decimal MinimumExcessAmountForSectionA { get; set; }
        public decimal MinimumExcessAmountForSectionB { get; set; }
        public decimal MinimumExcessAmountForSectionC { get; set; }
        public decimal MinimumExcessRateForSectionA { get; set; }
        public decimal MinimumExcessRateForSectionB { get; set; }
        public decimal MinimumExcessRateForSectionC { get; set; }
        //[Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }

        [Range(0.001, 100, ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal? BasicPremiumRateForSectionA { get; set; }
        public decimal? BasicPremiumRateForSectionB { get; set; }
        public decimal? BasicPremiumRateForSectionC { get; set; }
        public decimal ExcessAmount { get; set; }
        public decimal ExcessRate { get; set; }
        public List<EndorseMaterialOfInsurance> AddedMaterialsOfInsurance { get; set; }
        public List<EndorseMaterialOfInsurance> UpdatedMaterialsOfInsurance { get; set; }
        public List<EndorseMaterialOfInsurance> DiscontinuedMaterialsOfInsurance { get; set; }
        public string SituationOfRisk { get; set; }
        public string DeductiblesText { get; set; }
        public List<string> ElectronicEquipmentEndorsementList { get; set; }
        public string[] Locations { get; set; }
        public string Name { get; set; }
        public decimal? Megawatt { get; set; }
        
        public string Province { get; set; }
       
        public string District { get; set; }
       
        public string Municipality { get; set; }
       
        public string Ward { get; set; }
       
        public string StreetAddress { get; set; }
    }
    public class MaterialOfInsurance
    {
        public string SN { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Section { get; set; }
        public decimal UnitValue { get; set; }
        public decimal FullUnitValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal SumInsured { get; set; }
        public decimal FullSumInsured { get; set; }
        public string Identifier { get; set; } = Guid.NewGuid().ToString();
        
    }

    public class EndorseMaterialOfInsurance : MaterialOfInsurance
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }

    public class ExcessData
    {
        public string Section { get; set; }
        public decimal SumInsured { get; set; }
        public decimal ExcessRate { get; set; }
        public decimal ExcessAmount { get; set; }
    }
}
