using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.EndorsementData
{
    public class CalculateEndorsementPremiumViewModel
    {
        public decimal EndorsedBasicPremium { get; set; }
        public decimal EndorsedTPL { get; set; }
        public decimal EndorsedRSMDT { get; set; }
        public decimal EndorsedStampDuty { get; set; }
        public decimal EndorsedVAT { get; set; }
        public decimal VatPercent { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal NetPremium { get; set; }
        public decimal TaxablePremium { get; set; }
        public decimal TotalPremium { get; set; }
        public bool IsVatEnabled { get; set; }
        public decimal AgentCommissonRate { get; set; }
        public decimal SumInsured { get; set; }
        public decimal AgentCommissonAmount { get; set; }
        public decimal NepalAgentTDSRate { get; set; }
        public decimal NepalAgentTDSAmount { get; set; }
        public int? AgentId { get; set; }
        public decimal DirectDiscountRate { get; set; }
        //[Column(TypeName = "decimal(15, 4)")]
        public decimal DirectDiscountAmount { get; set; }
        public string Class { get; set; }
        public decimal PAAmount { get; set; }
        public decimal FurutrePremium { get; set; }
    }
}
