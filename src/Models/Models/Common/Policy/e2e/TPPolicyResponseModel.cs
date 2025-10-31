using System;

namespace Models.Common.Policy.e2e
{
    public class TPPolicyResponseModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? AgentId { get; set; }
        public string DofoId { get; set; }
        public string ClassId { get; set; }
        public string Branch { get; set; }
        public decimal NetPremium { get; set; }
        public string VehicleNo { get; set; }
        public string PartyCode { get; set; }
        public string InsuredName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; } //Full address
    }
}