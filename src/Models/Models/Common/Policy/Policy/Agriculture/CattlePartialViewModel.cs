using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CattlePartialViewModel : CommonNomineeModel, ILocation
    {
        public List<CattleInsuredPersons> CattleInsuredPersons { get; set; }
        [Required]
        public string CattleType { get; set; }
        public string CattleTypeName { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public string LandProvince { get; set; }
        public string LandDistrict { get; set; }
        public string LandMunicipality { get; set; }
        public string LandWard { get; set; }
        public string LandStreetAddress { get; set; }
        [RegularExpression("^[0-9]+(\\.[0-9]{1,5})?$", ErrorMessage = ("Must be a valid decimal number. Value must not exceed 5 decimal point"))]
        public string LandArea { get; set; }
        public string KittaNumber { get; set; }
        public string SerialNumberByLocalBody { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        [Range(0.001, 100, ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal? BasicPremiumRate { get; set; }
        public decimal? CorporateDiscountRate { get; set; }
        public List<IndividualCattleInformation> Cattles { get; set; }
        public bool IsInternal { get; set; }
        public string[] Locations { get; set; }

        public string Name { get; set; }
        public string LoanAmount { get; set; }
        public string LoanDuration { get; set; }
        #region Non Residential Nepali
        public string NonResidentIdentificationDocumentType { get; set; } 
        public string NonResidentIdentificationDocumentNumber { get; set; } 
        public DateTime? NonResidentIdentificationDocumentIssueDate { get; set; }
        public string NonResidentIdentificationDocumentIssuedBy { get; set; }
        #endregion
        #region Fish
        public int NumberOfFishes { get; set; }
        public decimal NumberOfFishesDecimal { get; set; }
        public string FishType { get; set; }
        public string[] FishTypeJson { get; set; }
        public string FishAge { get; set; }
        public string FishFarmingMethod { get; set; }
        public string FishFarmingTechinque { get; set; }
        public string FishFarmingTechinqueNepali { get; set; }
        public bool IsNormalFarmingUsed { get; set; }
        public decimal OldFishSumInsured { get; set; }
        public decimal FishSumInsured { get; set; }
        public decimal? FishPremiumRate { get; set; }
        public decimal? SpecialFarmingTechniqueRate { get; set; }
        
        public string CorporateRegistrationNumber { get; set; }
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
        public string ProposerPhoneNumber{ get; set; }
        public string ProposerMobileNumber{ get; set; }

        public string PondArea {get;set;}
        public string InsuranceCriteria {get;set;}
        public string InsuranceCriteriaNepali {get;set;}
        public bool IsPondInsuredForProductionCost {get;set;}
        public decimal? PondPremiumRate { get; set; }
        public decimal PondSumInsured { get; set; }

        public string Endorsement { get; set; }
        
        public List<FishInsuredPersons> InsuredPersons { get; set; }
        //ALIS
        public decimal TotalInsuredPerson { get; set; }
        public decimal SumInsuredAmount { get; set; }

        #endregion
    }
    public class IndividualCattleInformation
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Height { get; set; }
        public string Color { get; set; }
        public string BreedType { get; set; }
        public string Detail { get; set; }
        public decimal? NCDRate { get; set; }
        public string HealthStatus { get; set; }
        public string IdentificationCode { get; set; }
        public string TagId { get; set; }
        public decimal SumInsured { get; set; }
        public decimal BasicPremium { get; set; }
        
    }

    public class IndividualEndorsementCattleInformation
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Height { get; set; }
        public string Color { get; set; }
        public string BreedType { get; set; }
        public string Detail { get; set; }
        public decimal? NCDRate { get; set; }
        public string HealthStatus { get; set; }
        public string IdentificationCode { get; set; }
        public string TagId { get; set; }
        public decimal SumInsured { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal TransactionBasicPremium { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsNew { get; set; }
    }

    public class CattleEndorsementPartialViewModel : CattlePartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
        public List<IndividualEndorsementCattleInformation> AddedCattles { get; set; }
        public List<IndividualEndorsementCattleInformation> UpdatedCattles { get; set; }
        public List<IndividualEndorsementCattleInformation> DiscontinuedCattles { get; set; }    
        public List<FishInsuredPersons> AddedFishInsuredPersons { get; set; }
        public List<FishInsuredPersons> UpdatedFishInsuredPersons { get; set; }
        public List<FishInsuredPersons> DiscontinuedFishInsuredPersons { get; set; }
        public List<FishInsuredPersons> InsuredPersons { get; set; }
        public List<CattleInsured> UpdatedInsuredCattles { get; set; }
        [Range(0,100)]
        public decimal RefundSubsidyRate { get; set; }
        public decimal RefundSubsidyAmount { get; set; }
    }
    
    public class FishInsuredPersons
    {
        public string SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string InsuredPolicyArea { get; set; }
        public string InsuredFishType { get; set; }
        public decimal PASumInsured { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
    }
    public class CattleInsured
    {
        public string SN { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string InsuredPolicyArea { get; set; }
        public string InsuredCattleType { get; set; }
        public decimal PASumInsured { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
    }
    public class CattleInsuredPersons
    {
        public string SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string InsuredPolicyArea { get; set; }
        public string InsuredCattleType { get; set; }
        public decimal PASumInsured { get; set; }
        public string Remarks { get; set; }

    }
}
