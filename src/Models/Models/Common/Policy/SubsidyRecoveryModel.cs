using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class SubsidyRecoveryModel
    {
        public PremiumProperties PreviousCalculation { get; set; }
        public PremiumProperties CurrentCalculation { get; set; }
        public string PortfolioAlias { get; set; }
        public string PolicyNumber { get; set; }
        public string SubsidyAdjustedBy { get; set; }
    }

    public class PremiumProperties
    {
        public decimal SumInsured { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal SubsidyAmount { get; set; }
        public decimal ClientPayable { get; set; }
        public decimal PAOfInsured { get; set; }
        public decimal StampAmount { get; set; }
        public decimal SubsidyRate { get; set; }
        public string InsuredParty { get; set; }
    }
}
