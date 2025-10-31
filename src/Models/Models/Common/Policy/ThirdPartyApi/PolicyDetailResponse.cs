using Models.Common.Policy.Policy;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class PolicyDetailResponse
    {
        public string Class { get; set; }
        public string ClassId { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string DraftNumber { get; set; }
        public string PartyCode { get; set; }
        public string Portfolio { get; set; }
        public string BillNo { get; set; }
        public string ReceiptNo { get; set; }
        public string InsuredPartyName { get; set; }
        public decimal Transaction { get; set; }
        public decimal TransactionBasicPremium { get; set; }
        public decimal TransactionPoolPremium { get; set; }
        public decimal TransactionThirdPartyPremium { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public decimal TransactionTaxableAmount { get; set; }
        public decimal TransactionTax { get; set; }
        public CreatePolicyViewModel PolicyModel { get; set; }
    }
}