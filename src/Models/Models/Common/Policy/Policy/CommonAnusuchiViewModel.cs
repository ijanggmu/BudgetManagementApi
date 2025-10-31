using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class CommonAnusuchiViewModel
    {
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
    }
}
