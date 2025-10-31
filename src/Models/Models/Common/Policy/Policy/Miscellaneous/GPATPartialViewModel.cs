using System;
using System.Collections.Generic;
using Models.Common.Policy.Policy.GPA;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class GPATPartialViewModel:CommonAnusuchiViewModel
    {
        public List<NamedInsuredPerson> NamedInsuredPersons { get; set; }
        public List<UnNamedInsuredPerson> UnNamedInsuredPersons { get; set; }

        public decimal LessGroupDiscountRate { get; set; }
        public string ConfirmationDetails { get; set; }
        public decimal FreshBasicPremium { get; set; }
        public decimal FreshRSMDT { get; set; }
        public decimal EndorsedBasicPremium { get; set; }
        public decimal EndorsedRSMDT { get; set; }
        public decimal LessDirectDiscountRate { get; set; }
        public string TrekkingArea { get; set; }
        public decimal? ShortScaleRate { get; set; }
        public List<ClassificationRiskCoverageRateViewModel> ClassificationRiskCoverageRateViewModels { get; set; }

        //new properties
        public decimal BasicPremiumRate { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }
        public decimal AdditionalRiskRate { get; set; }
        public string CompulsoryExcess { get; set; }
        public bool IsGroupDiscount { get; set; }
    }

    public class NamedInsuredPerson : InsuredPersonBase
    {
        public string Name { get; set; }
        public decimal ABCDBasicPremium { get; set; }
        public decimal EPremium { get; set; }
        public decimal RSMDTAmount { get; set; }

    }

    public class UnNamedInsuredPerson : InsuredPersonBase
    {
        public int NumberofPerson { get; set; }
        public int ChangeInCount { get; set; }
        public decimal Rate { get; set; }
    }


    public class InsuredPersonBase
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int SN { get; set; }
        public string Occupation { get; set; }
        public int Age { get; set; }
        public string ParentId { get; set; }
        public decimal SumInsured { get; set; }
        public decimal Pa { get; set; }
        public decimal Medical { get; set; }
        public decimal RSMDT { get; set; }
        public string PrimaryId { get; set; }
        public decimal MedicalBenefitSumInsured { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsAdded { get; set; }

        public string Classification { get; set; }
        public bool IsABCSelected { get; set; }
        public bool IsDSelected { get; set; }
        public bool IsESelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool ISAbove1900Ft { get; set; }

        //new fields
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string Designation { get; set; }
        public bool IsAdditionalRiskDetailEdited { get; set; }
        public decimal? SIForPrint { get; set; }

    }


    #region Endorsement

    public class EndorsedInsuredPerson : InsuredPersonBase
    {
        public decimal ABCDBasicPremium { get; set; }
        public decimal EPremium { get; set; }
        public decimal GroupDicount { get; set; }
        public decimal DirectDiscount { get; set; }
        public decimal RSMDTAmount { get; set; }
    }


    public class EndorsedNamedInsuredPerson : EndorsedInsuredPerson
    {
        public string Name { get; set; }
    }
    public class EndorsedUnNamedInsuredPerson : EndorsedInsuredPerson
    {
        public int NumberofPerson { get; set; }
        public int ChangeInCount { get; set; }
        public decimal Rate { get; set; }
    }

    public class GPATEndorsementPartialViewModel : GPATPartialViewModel
    {
        public List<EndorsedNamedInsuredPerson> AddedNamedPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> UpdatedNamedPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> DiscontinuedNamedPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> AddedUnNamedPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> UpdatedUnNamedPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> DiscontinuedUnNamedPersons { get; set; }
    }

    #endregion
}
