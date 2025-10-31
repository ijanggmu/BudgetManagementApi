using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class MoneyDetailcalculationResult : CommonMiscellaneousDetailsProperty
    {
        public CalculationSubDetailAmountModel CashInSafeBasicPremiumA { get; set; }
        public CalculationSubDetailAmountModel CashInSafeBasicPremium { get; set; }
        public CalculationSubDetailAmountModel CashInSafeBasicPremiumB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremiumA { get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremium{ get; set; }
        public CalculationSubDetailAmountModel CashInCounterBasicPremium{ get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremiumB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremiumC { get; set; }
        public CalculationSubDetailAmountModel CashInSafeRSMDA { get; set; }
        public CalculationSubDetailAmountModel CashInSafeRSMD { get; set; }
        public CalculationSubDetailAmountModel CashInSafeRSMDB { get; set; }
        public CalculationSubDetailAmountModel CashInCounterRSMD { get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMDA { get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMD{ get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMDB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMDC { get; set; }
        public CalculationSubDetailAmountModel CashInSafeTerrorism{ get; set; }
        public CalculationSubDetailAmountModel CashInCounterTerrorism{ get; set; }
        public CalculationSubDetailAmountModel IncludingProperty{ get; set; }
        public CalculationSubDetailAmountModel CashInSafeTerrorismA { get; set; }
        public CalculationSubDetailAmountModel CashInSafeTerrorismB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorismA { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorism { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorismB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorismC { get; set; }

    }
}
