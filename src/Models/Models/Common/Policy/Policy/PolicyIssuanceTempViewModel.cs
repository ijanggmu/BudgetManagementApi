using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class PolicyIssuanceTempViewModel
    {
        public string Id { get; set; }
        public int SN { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string PolicyType { get; set; }
        public string DocNo { get; set; }
        public int? DraftNo { get; set; }
        public string BillNo { get; set; }
        public string ReceiptNo { get; set; }
        public string FiscalYear { get; set; }
        public string AccountStatus { get; set; }
        //Class and Doc Info
        public string Class { get; set; }
        public string DocumentType { get; set; }
        public string EndorsementType { get; set; }
        public int? EndorsementTypeInt { get; set; }
        //Policy Dates
        public DateTime ProposedDate { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string PolicyPeriod { get; set; }
        //Party Info
        public string InsuredParty { get; set; }
        //province/district/city/ward/streetaddress stored in json
        public string AddressInfo { get; set; }
        public string PhoneNumber { get; set; }
        //public string PartySearch { get; set; }
        //public string ACParty { get; set; }
        //Premium Amounts
        public decimal TotalSumInsuredAmount { get; set; }
        public decimal BasicPremiumamount { get; set; }
        public decimal PoolPremiumAmount { get; set; }
        public decimal ThirdPartyPremium { get; set; }
        public decimal SumInsured { get; set; }
        public decimal NetPremium { get; set; }
        public string Stampduty { get; set; }
        public string VATBillNo { get; set; }
        public decimal TotalPremium { get; set; }
        //DO/AgentInfo
        public string Agent { get; set; }
        public string FieldOfficier { get; set; }
        public string AgentCommision { get; set; }
        public string Branch { get; set; }
        public string ClassJson { get; set; }
        public string PartyId { get; set; }
        public string ClassId { get; set; }
        public MotorPartialViewModel MyProperty { get; set; }
        public string InsuredPartyName { get; set; }
        public bool IsPrinted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string BranchCode { get; set; }
        public bool ApplyGovtConfig { get; set; }

        #region Used In GPA Rescue
        public decimal SIAmountForMedicalBenefitUptoBaseCamp { get; set; }
        public decimal PremiumAmountForMedicalBenefitUptoBaseCamp { get; set; }
        public decimal SIAmountForMedicalBenefitAbove1900ft { get; set; }
        public decimal PremiumAmountForMedicalBenefitAbove1900ft { get; set; }
        #endregion
    }
}
