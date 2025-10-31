using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models.Common.Policy.Endorsement;

namespace Models.Common.Policy.EndorsementData
{
    public class CreateEndorsementDataViewModel : EndorsementViewModel
    {
        public int SN { get; set; }

        //[DisplayName("Policy Number")]
        //public string PolicyNumber { get; set; }
        public string PolicyId { get; set; }
        //[DisplayName("Document Number")]
        //public string DocumentNumber { get; set; }
        //[DisplayName("Endorsement Date")]
        //public DateTime EndorsementDate { get; set; }
        [DisplayName("Endorsement Type")]
        public bool IsEndorsementType { get; set; }
        [DisplayName("Selected Document Number")]
        public string SelectedDocumentNumber { get; set; }
        [DisplayName("Endorsement Amount")]
        public decimal EndorsementAmt { get; set; }
        [DisplayName("Name")]
        public string NameCorporate { get; set; }
        [DisplayName("Name in Nepali")]
        public string NameCorporateNepali { get; set; }
        [DisplayName("Address")]
        public string AddressCorporate { get; set; }
        [DisplayName("PAN Number")]
        public string PANCorporate { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneCorporate { get; set; }
        public string TypeOfCompany { get; set; }
        [DisplayName("First Name")]
        public string FirstNameIndividual { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleNameIndividual { get; set; }
        [DisplayName("Last Name")]
        public string LastNameIndividual { get; set; }
        [DisplayName("FirstName In Nepali")]
        public string FirstNameIndividualNp { get; set; }
        [DisplayName("MiddleName In Nepali")]
        public string MiddleNameIndividualNp { get; set; }
        [DisplayName("LastName In Nepali")]
        public string LastNameIndividualNp { get; set; }
        [DisplayName("Address")]
        public string AddressIndividual { get; set; }
        [DisplayName("PAN Number")]
        public string PANIndividual { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneIndividual { get; set; }
        [DisplayName("Payment")]
        public string EndorsementPayment { get; set; }
        [DisplayName("Basic Premium")]
        public decimal EndorsedBasicPremium { get; set; }
        [DisplayName("TPL")]
        public decimal EndorsedTPL { get; set; }
        [DisplayName("RSMDT")]
        public decimal EndorsedRSMDT { get; set; }
        [DisplayName("Net Premium")]
        public decimal EndorsedNetPremium { get; set; }
        [DisplayName("VAT Amount")]
        public decimal VATAmount { get; set; }
        [DisplayName("Stamp Duty")]
        public decimal StampDuty { get; set; }
        //[DisplayName("Sum Insured")]
        //public decimal SumInsured { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public decimal OldSumInsured { get; set; }
        public string EndorsementParty { get; set; }
        public string EndorsementPortfolio { get; set; }
        public string EndorsementPortfolioParent { get; set; }
        public string EndorsementPortfolioAlias { get; set; }
        [DisplayName("Endorsement Type")]
        [Required]
        public int EndorsementTypeList { get; set; }
        //[Required]
        //public string Class { get; set; }
        public string VegetablePartial { get; set; }
        public bool IsVatEnabled { get; set; }
        public string Remarks { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal PAAmount { get; set; }
        public string CattlesPartial { get; set; }
        public string PoultriesPartial { get; set; }
        public int MigrationStatus { get; set; }
        public int ManageEndorsementType { get; set; }
        public string PortfolioForDropdown { get; set; }
    }
}
