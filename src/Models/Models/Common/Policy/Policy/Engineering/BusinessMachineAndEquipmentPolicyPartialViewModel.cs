using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class BusinessMachineAndEquipmentPolicyPartialViewModel : ILocation
    {
        public List<BusinessMachineMaterialOfInsurance> MaterialsOfInsurance { get; set; }
        public List<EndorseBusinessMachineMaterialOfInsurance> AddedMaterialsOfInsurance { get; set; }
        public List<EndorseBusinessMachineMaterialOfInsurance> UpdatedMaterialsOfInsurance { get; set; }
        public List<EndorseBusinessMachineMaterialOfInsurance> DiscontinuedMaterialsOfInsurance { get; set; }
        //[Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        
        public string Province { get; set; }
       
        public string District { get; set; }
       
        public string Municipality { get; set; }
       
        public string Ward { get; set; }
       
        public string StreetAddress { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        //public bool IsTPLSelected { get; set; }
        public string BusinessOccupation { get; set; }
        public string ThePremises { get; set; }
        public string SituationOfRisk { get; set; }
        public string DeductibleRate { get; set; }
        public string DeductibleMinimum { get; set; }
        public string DeductiblesText { get; set; }
        public List<string> Endorsements { get; set; }
        public bool ProvideSpecialDiscount { get; set; }
        [Range(0.001, 100, ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal BasicPremiumRate { get; set; }
        public string[] Locations { get; set; }

        public string Name { get; set; }
        public decimal? Megawatt { get; set; }
    }

    public class BusinessMachineMaterialOfInsurance
    {
        public string SN { get; set; }
        public int ItemNo { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public decimal FullSuminsured { get; set; }
        public string Identifier { get; set; }
    }

    public class EndorseBusinessMachineMaterialOfInsurance : BusinessMachineMaterialOfInsurance
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}
