using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class VegetablesPartialViewModel : CommonNomineeModel, ILocation
    {
        //ALIS
        public decimal DirectDiscountRate { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal TotalInsuredPerson { get; set; }
        public List<InsuredProperty> InsuredProperty { get; set; }
        public List<LandAreaInsurance> LandAreaOfInsurance { get; set; }
        public List<VegetableInsuredPersons> InsuredPersons { get; set; }
        [Required]
        public string VegetableName { get; set; }
        public string LandProvince { get; set; }
        public string LandDistrict { get; set; }
        public string LandMunicipality { get; set; }
        public string LandWard { get; set; }
        public string LandStreetAddress { get; set; }
        public string TotalArea { get; set; }
        public string VegetableType { get; set; }
        public string Remarks { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public decimal? CorporateDiscountRate { get; set; }
        public decimal? BasicPremiumRate { get; set; }
        [RegularExpression("^[0-9+-]*$", ErrorMessage = ("Must be a valid number"))]

        public string NumberofPlants { get; set; }
        public string NumberOfPlantsInsured { get; set; }
        public string KittaNumber { get; set; }
        public string VegetableKind { get; set; }
        public string VegInsuranceType { get; set; }
        public decimal PlantSumInsured { get; set; }
        public decimal FruitSumInsured { get; set; }
        public string[] Locations { get; set; }
        public string Name { get; set; }
        public string NumberAssignedFromLocalGovernment{ get; set; }
        public decimal? LoanAmount { get; set; }
        public string LoanDuration { get; set; }
        
        #region Non Residential Nepali
        public string NonResidentIdentificationDocumentType { get; set; } 
        public string NonResidentIdentificationDocumentNumber { get; set; } 
        public DateTime? NonResidentIdentificationDocumentIssueDate { get; set; }
        public string NonResidentIdentificationDocumentIssuedBy { get; set; }
        #endregion
        
        #region Cardamom
        [Required(ErrorMessage = "Please enter the sum insured for each cardamom plant")]
        public decimal PerCardamomSumInsured { get; set; }
        [Required(ErrorMessage = "Please enter the number of cardamom plants")]
        public int NumberOfCardamom { get; set; }
        #endregion

        #region Mushroom
        [Required(ErrorMessage = "Please enter the number of mushroom bulbs")]
        public int NumberOfMushroomBulb { get; set; }
        [Required(ErrorMessage = "Please enter the cost of production material")]
        public decimal CostOfProductionMaterial { get; set; }
        [Required(ErrorMessage = "Please enter the cost of machinery")]
        public decimal CostOfMachinery { get; set; }
        [Required(ErrorMessage = "Please enter the cost of management expenses")]
        public decimal ManagementExpenses { get; set; }
        #endregion

        #region Seed
        [Required(ErrorMessage = "Please enter the name of seed")]
        public string NameOfSeed { get; set; }
        [Required(ErrorMessage = "Please enter the amount of production quantity per hector")]
        public decimal ProductionQuantityPerHector { get; set; }
        [Required(ErrorMessage = "Please enter the cost per ton")]
        public decimal ExactCostPerTon { get; set; }
        #endregion

        #region Bee
        public int NumberOfBeeHives { get; set; }
        public decimal PurchaseCostPerBeeHiveWithStand { get; set; }
        public decimal CostPerBeeHive { get; set; }
        public decimal AverageBeekeepingManagementExpensesPerBeeHive { get; set; }
        #endregion

        #region Chaityadhan
        public string InsuredThrough { get; set; }
        #endregion
        
        #region Kiwi
        public string AgeOfPlant { get; set; }
        public string PlantType { get; set; }
        #endregion

       
    }
    public class LandAreaInsurance
    {
        public string SN { get; set; }
        public string Id { get; set; }
        public string KittaNo { get; set; }
        public decimal Area { get; set; }
        public decimal SumInsured { get; set; }
    }

    public class LandAreaInsuranceEndorse
    {
        public string SN { get; set; }
        public string Id { get; set; }
        public string KittaNo { get; set; }
        public decimal Area { get; set; }
        public decimal SumInsured { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsNew { get; set; }
    }

    public class VegetableInsuredPersonEndorse
    {
        public string SN { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string InsuredPolicyArea { get; set; }
        public string InsuredType { get; set; }
        public decimal PASumInsured { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsNew { get; set; }
    }

    public class VegetableEndorsementPartialViewModel: VegetablesPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
        public List<LandAreaInsuranceEndorse> AddedLandAreaInsurance { get; set; }
        public List<LandAreaInsuranceEndorse> UpdatedLandAreaInsurance { get; set; }
        public List<LandAreaInsuranceEndorse> DiscontinuedLandAreaInsurance { get; set; }
        public List<VegetableInsuredPersonEndorse> UpdatedInsuredPerson { get; set; }
        public decimal RefundSubsidyRate { get; set; }
        public decimal RefundSubsidyAmount { get; set; }

    }
    
    public class VegetableInsuredPersons
    {
        public string SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string InsuredPolicyArea { get; set; }
        public string InsuredType { get; set; }
        public decimal PASumInsured { get; set; }
        public string Remarks { get; set; }

    }
}
