using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class BoilerPolicyPartialViewModel
    {
        public List<BoilerAndPressurePlant> BoilerAndPressurePlants { get; set; }
        public List<SurroundingPropertyOfInsured> SurroundingPropertiesOfInsured { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsTPLSelected { get; set; }
        
        public string Province { get; set; }
       
        public string District { get; set; }
       
        public string Municipality { get; set; }
       
        public string Ward { get; set; }
       
        public string StreetAddress { get; set; }
        public decimal? BodilyInjury { get; set; }
        public decimal? PropertyDamage { get; set; }
        //[Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool ProvideSpecialDiscount { get; set; }

        [Range(0.001, 100, ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal? BasicPremiumRate { get; set; }
        public string ConfirmationDetails { get; set; }
        public decimal? ExcessAmount { get; set; }
        public decimal? ExcessRate { get; set; }
        public string SituationOfRisk { get; set; }
        public decimal? Megawatt { get; set; }
        public List<EndorsedBoilerAndPressurePlant> AddedBoilerAndPressurePlants { get; set; }
        public List<EndorsedSurroundingPropertyOfInsured> AddedSurroundingPropertiesOfInsured { get; set; }
        public List<EndorsedBoilerAndPressurePlant> UpdatedBoilerAndPressurePlants { get; set; }
        public List<EndorsedSurroundingPropertyOfInsured> UpdatedSurroundingPropertiesOfInsured { get; set; }
        public List<EndorsedBoilerAndPressurePlant> DiscontinuedBoilerAndPressurePlants { get; set; }
        public List<EndorsedSurroundingPropertyOfInsured> DiscontinuedSurroundingPropertiesOfInsured { get; set; }
    }
    public class BoilerAndPressurePlant : SurroundingPropertyOfInsured
    {
        public string Location { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? YearOfMake { get; set; }
    }
    public class SurroundingPropertyOfInsured
    {
        public string Identifier { get; set; } = Guid.NewGuid().ToString();
        public string SN { get; set; }
        public string MakersNameOrCapacity { get; set; }
        public decimal Value { get; set; }
        public decimal FullSumInsured { get; set; }
    }

    public class EndorsedBoilerAndPressurePlant : BoilerAndPressurePlant
    {
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
    
    public class EndorsedSurroundingPropertyOfInsured : SurroundingPropertyOfInsured
    {
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }

}
