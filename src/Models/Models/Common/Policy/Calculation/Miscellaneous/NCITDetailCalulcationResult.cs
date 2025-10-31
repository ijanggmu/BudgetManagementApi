namespace Models.Common.Policy.Calculation.Miscellaneous
{
    public class NCITDetailCalulcationResult:CommonMiscellaneousDetailsProperty
    {
        public CalculationSubDetailAmountModel CashInSafeBasicPremiumA { get; set; }
        public CalculationSubDetailAmountModel CashInSafeBasicPremiumB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremiumA { get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremiumB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitBasicPremiumC { get; set; }
        public CalculationSubDetailAmountModel CashInSafeRSMDA { get; set; }
        public CalculationSubDetailAmountModel CashInSafeRSMDB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMDA { get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMDB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitRSMDC { get; set; }
        public CalculationSubDetailAmountModel CashInSafeTerrorismA { get; set; }
        public CalculationSubDetailAmountModel CashInSafeTerrorismB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorismA { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorismB { get; set; }
        public CalculationSubDetailAmountModel CashInTransitTerrorismC { get; set; }
    }
}