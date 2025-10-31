using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CerealsPartialViewModel : CommonNomineeModel
    {
        //added alis
        public decimal DirectDiscountRate { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal TotalInsuredPerson { get; set; }

    public List<InsuredPersons> InsuredPersons { get; set; }

        public string CerealName { get; set; }
        
        public string CorporateRegistrationNumber { get; set; }
        public string CorporatePANNumber { get; set; }
        public string CorporateRegistrationPlace { get; set; }
        public string IdentificationIssueDate { get; set; }
        public string FirmRegistrationAddress { get; set; }
        [Required]
        public string ProposerProvince { get; set; }
        public string ProposerDistrict { get; set; }
        public string ProposerMunicipality { get; set; }
        public string ProposerWard { get; set; }
        public string ProposerStreetAddress { get; set; }
        public string ProposerCity { get; set; }
        public string ProposerEmail { get; set; }
        public string ProposerPhoneNumber{ get; set; }
        [Required] 
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string ProposerMobileNumber{ get; set; }

        public string CerealKind { get; set; }
        public string ProposedCerealType { get; set; }
        public string CerealPlantationType { get; set; }
        public string CerealPaddyType { get; set; }
        [Required]
        public DateTime? CerealPlantationDate { get; set; }
        [RegularExpression("^[0-9]+(\\.[0-9]{1,5})?$", ErrorMessage = ("Must be a valid decimal number. Value must not be exceed 5 decimal point"))]
        public decimal CerealPlantationArea { get; set; }
        [RegularExpression("^[0-9]+(\\.[0-9]{1,5})?$", ErrorMessage = ("Must be a valid decimal number. Value must not be exceed 5 decimal point"))]
        public decimal ProductivityOfDistrict { get; set; }
        [RegularExpression("^[0-9+-]*$", ErrorMessage = ("Must be a valid number"))]
        public decimal MarketValueByAgriculturalDepartment { get; set; }
        
        [Required]
        //Cereal plantation location
        public string LandProvince { get; set; }
        public string LandDistrict { get; set; }
        public string LandMunicipality { get; set; }
        public string LandWard { get; set; }
        public string LandStreetAddress { get; set; }
        [Required]
        public string KittaNumber { get; set; }
        public string Tole { get; set; }
        public string NumberAssignedFromLocalGovernment{ get; set; }

        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public decimal? CorporateDiscountRate { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        [RegularExpression("^[0-9]+(\\.[0-9]{1,8})?$", ErrorMessage = ("Must be a valid number"))]
        public decimal? LoanAmount { get; set; }
        public string LoanDuration { get; set; }
        
        #region Non Residential Nepali
        public string NonResidentIdentificationDocumentType { get; set; } 
        public string NonResidentIdentificationDocumentNumber { get; set; } 
        public DateTime? NonResidentIdentificationDocumentIssueDate { get; set; }
        public string NonResidentIdentificationDocumentIssuedBy { get; set; }
        #endregion
    }

    public class CerealEndorsementPartialViewModel: CerealsPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
        public List<InsuredPersons> AddedInsuredPersons { get; set; }
        public List<InsuredPersons> UpdatedInsuredPersons { get; set; }
        public List<InsuredPersons> DiscontinuedInsuredPersons { get; set; }
        public List<InsuredPersons> InsuredPersons { get; set; }
        [Range(0,100)]
        public decimal RefundSubsidyRate { get; set; }
        public decimal RefundSubsidyAmount { get; set; }

    }   
    public class InsuredPersons
    {
        public string SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string InsuredPolicyArea { get; set; }
        public string InsuredCerealType { get; set; }
        public decimal PASumInsured { get; set; }
        public string Remarks { get; set; }

    }
}
