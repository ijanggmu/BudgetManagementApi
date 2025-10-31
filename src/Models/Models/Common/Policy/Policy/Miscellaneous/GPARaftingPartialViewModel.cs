using System;
using System.Collections.Generic;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class GPARaftingPartialViewModel:CommonAnusuchiViewModel
    {
        public List<NamedInsuredPerson> NamedInsuredPersons { get; set; }
        public List<UnNamedInsuredPerson> UnNamedInsuredPersons { get; set; }
        public string RaftingArea { get; set; }
        public string ConfirmationDetails { get; set; }
       
        public decimal LessGroupDiscountRate { get; set; }
        public decimal LessDirectDiscountRate { get; set; }
        public decimal? ShortScaleRate { get; set; }
        public List<ClassificationRiskCoverageRateViewModel> ClassificationRiskCoverageRateViewModels { get; set; }
        // added fields
        public string CompulsoryExcess { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }
        public decimal AdditionalRiskRate { get; set; }
        public bool IsGroupDiscount { get; set; }
    }

    public class GPARaftingEndorsementPartialViewModel : GPARaftingPartialViewModel
    {
        public List<EndorsedNamedInsuredPerson> AddedNamedPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> UpdatedNamedPersons { get; set; }
        public List<EndorsedNamedInsuredPerson> DiscontinuedNamedPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> AddedUnNamedPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> UpdatedUnNamedPersons { get; set; }
        public List<EndorsedUnNamedInsuredPerson> DiscontinuedUnNamedPersons { get; set; }
    }
}
