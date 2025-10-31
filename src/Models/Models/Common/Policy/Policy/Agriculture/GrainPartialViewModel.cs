using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class GrainPartialViewModel : CommonNomineeModel
    {
        //ALIS
        public decimal DirectDiscountRate { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal TotalInsuredPerson { get; set; }
        public List<InsuredProperty> InsuredProperty { get; set; }

        //END ALIS

        public string GrainName { get; set; }
        public string CorporateRegistrationNumber { get; set; }
        public string CorporatePANNumber { get; set; }
        public string CorporateRegistrationPlace { get; set; }
        public string IdentificationIssueDate { get; set; }
        public string FirmRegistrationAddress { get; set; }
         public string ProposerProvince { get; set; }
        public string ProposerDistrict { get; set; }
        public string ProposerMunicipality { get; set; }
        public string ProposerWard { get; set; }
        public string ProposerStreetAddress { get; set; }
        public string ProposerCity { get; set; }
        public string ProposerEmail { get; set; }
        public string ProposerPhoneNumber { get; set; }
        public string ProposerMobileNumber { get; set; }
        public string GrainKind { get; set; }
        public string ProposedGrainType { get; set; }
        public string GrainPlantationType { get; set; }
        public string GrainPaddyType { get; set; }
        public DateTime? GrainPlantationDate { get; set; }
        public decimal GrainPlantationArea { get; set; }
        public decimal ProductivityOfDistrict { get; set; }
        public decimal MarketValueByAgriculturalDepartment { get; set; }
        public string LandProvince { get; set; }
        public string LandDistrict { get; set; }
        public string LandMunicipality { get; set; }
        public string LandWard { get; set; }
        public string LandStreetAddress { get; set; }
        public string KittaNumber { get; set; }
        public string Tole { get; set; }
        public string NumberAssignedFromLocalGovernment { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public decimal? CorporateDiscountRate { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public decimal? LoanAmount { get; set; }
        public string LoanDuration { get; set; }
    }

    public class InsuredProperty
    {
        //Veg-ALIS
        public string FarmingType { get; set; }
        public string SubSector { get; set; }
        public string PlantBreed { get; set; }
        public string TotalAreaHectare { get; set; }
        public string DistrictProductionPerHectare { get; set; }
        public string PricePerTon { get; set; }

        //Grain
        public string GrainName { get; set; }

        //pulses-ALIS
        public string CropName { get; set; }
        public string PlantPerHectare { get; set; }
        public string PreviousProductionPerHectare { get; set; }
    }
}