using System;
using System.Collections.Generic;

namespace Models.Common.Policy.Policy.GPA
{
    public class GPAPolicyPartialViewModel
    {
        public List<NamedInsuredPerson> NamedInsuredPersons { get; set; }
        public List<UnnamedInsuredPersons> UnnamedInsuredPersons { get; set; }
        public Dictionary<InsuredPersonClassification, RiskConfiguration> ClassificationWiseRiskConfiguration
        {
            get; set;
        }
        
        public decimal LessGroupDiscountRate { get; set; }
        public string ConfirmationDetails { get; set; }
        public decimal? ShortScaleRate { get; set; }
        public string EndorsementDate { get; set; }

        //new properties
        public decimal BasicPremiumRate { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }
        public decimal AdditionalRiskRate { get; set; }
        public string CompulsoryExcess { get; set; }
        public bool IsGroupDiscount { get; set; }

        //for GPA-Player
        public string GameCode { get; set; }
        public string GameRiskType { get; set; }
        public string GameDescription { get; set; }
        public decimal GameRiskRate { get; set; }
        public decimal MedicalSumInsuredLimit { get; set; }
        
        
        //for anusuchi 7
        public DateTime? AmendmentOfNameOfInsuredDate { get; set; }
        public string AmendmentOfNameOfInsuredExistingDetails { get; set; }
        public string AmendmentOfNameOfInsuredAmendedDetail { get; set; }
        public string AmendmentOfNameOfInsuredRemarks { get; set; }

        public DateTime? ChangeOfAddressOfInsuredDate { get; set; }
        public string ChangeOfAddressOfInsuredExistingDetails { get; set; }
        public string ChangeOfAddressOfInsuredAmendedDetail { get; set; }
        public string ChangeOfAddressOfInsuredRemarks { get; set; }

        public DateTime? ChangeInEmployeeListNoDate { get; set; }
        public string ChangeInEmployeeListNoExistingDetails { get; set; }
        public string ChangeInEmployeeListNoAmendedDetail { get; set; }
        public string ChangeInEmployeeListNoRemarks { get; set; }

        public DateTime? AdditionOfRiskDetailsDate { get; set; }
        public string AdditionOfRiskDetailsExistingDetails { get; set; }
        public string AdditionOfRiskDetailsAmendedDetail { get; set; }
        public string AdditionOfRiskDetailsRemarks { get; set; }

        public DateTime? CancellationOfPolicyDate { get; set; }
        public string CancellationOfPolicyExistingDetails { get; set; }
        public string CancellationOfPolicyAmendedDetail { get; set; }
        public string CancellationOfPolicyRemarks { get; set; }

        public DateTime? RefundOfPremiumDate { get; set; }
        public string RefundOfPremiumExistingDetails { get; set; }
        public string RefundOfPremiumAmendedDetail { get; set; }
        public string RefundOfPremiumRemarks { get; set; }
        //anusuchi5
        public string NamedDetails { get; set; }
        public string UnnamedDetails { get; set; }
        public int? GPAPolicyPeriod { get; set; }


       
    }

    public class GPAEndorsementPartialViewModel : GPAPolicyPartialViewModel
    {
        public List<EndorsedNamedInsuredPerson> AddedNamedPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> UpdatedNamedPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> DiscontinuedNamedPersons { get; set; }
        public List<EndorsedUnnamedInsuredPersons> AddedUnnamedPersons { get; set; }
        public List<EndorsedUnnamedInsuredPersons> UpdatedUnnamedPersons { get; set; }
        public List<EndorsedUnnamedInsuredPersons> DiscontinuedUnnamedPersons { get; set; }
        
    }
}
