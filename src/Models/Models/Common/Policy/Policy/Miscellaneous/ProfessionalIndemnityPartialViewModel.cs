using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class ProfessionalIndemnityPartialViewModel
    {
        [Required(ErrorMessage = "Days is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Please enter valid Number")]
        public int PolicyPeriodInDays { get; set; }

        [Required(ErrorMessage = "Business/Occupation is required")]
        public string BusinessOrOccupation { get; set; }

        [Required(ErrorMessage = "Address of business premises covered is required")]
        public string AddressOfBusinessPremisesCovered { get; set; }

        [Required(ErrorMessage = "Limit of Indemnity is required")]
        public decimal LimitOfIndemnity { get; set; }
        
        [Required(ErrorMessage = "Basic Rate is required")]
        public decimal BasicRate { get; set; }

        [Required(ErrorMessage = "Compulsary Excess is required")]
        public decimal CompulsaryExcess { get; set; }

        [Required(ErrorMessage = "Territorial Limits is required")]
        public string TerritorialLimits { get; set; } = "Within Nepal";

        [Required(ErrorMessage = "Name of Project is required")]
        public string NameOfProject { get; set; }

        [Required(ErrorMessage = "Contract Number is required")]
        public string ContractNo { get; set; }
         
        public string EndorsementText { get; set; }

        public DateTime ExtendedReportingPeriodStartDate { get; set; }
        public DateTime ExtendedReportingPeriodEndDate { get; set; }

    }

    public class ProfessionalIndemnityEndorsementPartialViewModel: ProfessionalIndemnityPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
        public decimal FutureSumInsured { get; set; }
    }
}
