using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy
{

    public class PolicyDetailInfoViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; }


        public string PolicyNumber { get; set; }
        public int DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public int EndrosementType { get; set; }

        public string Class { get; set; }
        public string ClassAlias { get; set; }
        public string Branch { get; set; }
        public string FiscalYear { get; set; }


        public decimal BasicSumInsuredAmount { get; set; }
        public decimal RsmdtSumInsuredAmount { get; set; }
        public decimal ThirdPartySumInsuredAmount { get; set; }
        public decimal BasicPremiumAmount { get; set; }
        public decimal RsmdtPremiumAmount { get; set; }
        public decimal ThirdpartyPremiumAmount { get; set; }


        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString();
        public bool IsDeleted { get; set; }


        public string PartyType { get; set; }
        public string PartyFullname { get; set; }

       
        public DateTime PolicyStart { get; set; }
        public DateTime PolicyEnd { get; set; }
        public string IssueDate { get; set; }
    }

    public class ClaimPolicyDetailViewModel
    {
        public PolicyDetailInfoViewModel ReinsuranceDetail { get; set; }

    }
}
