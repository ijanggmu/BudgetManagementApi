using Models.Common.Policy.Policy.Fire;

namespace Models.Common.Policy.Calculation.Fire
{
    public class FirePremiumCalculationModel
    {
        public FirePremiumCalculationModel(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
        public decimal TotalValueOfContents { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal BasicPremiumToDisplay { get; set; }
        public decimal ShortScaleBasicPremiumToDisplay { get; set; }
        public decimal MinimumBasicPremiumAmount { get; set; }
        public decimal ShortScaleMinimumBasicPremiumAmount { get; set; }
        public decimal ShortScaleBasicPremium { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal SecondaryBasicPremiumRate { get; set; }
        public decimal DirectDiscountAmount { get; set; }
        public decimal ShortScaleDirectDiscountAmount { get; set; }
        public decimal PremiumAfterDirectDiscount { get; set; }
        public decimal ShortScalePremiumAfterDirectDiscount { get; set; }
        public decimal RSMDTDirectDiscountAmount { get; set; }
        public decimal RSMDTShortScaleDirectDiscountAmount { get; set; }
        public decimal PremiumAfterRSMDTDirectDiscount { get; set; }
        public decimal ShortScaleRSMDTAfterDirectDiscount { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal RSMDTAmountToDisplay { get; set; }
        public decimal RSMDTActualDisplay { get; set; }
        public decimal ShortScaleRSMDTAmountToDisplay { get; set; }
        public decimal MinimumRSMDTAmount { get; set; }
        public decimal ShortScaleMinimumRSMDTAmount { get; set; }
        public decimal ShortScaleRSMDTAmount { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal ShortScaleGrossPremium { get; set; }
        public decimal RSMDAmount { get; set; }
        public decimal RSMDRate { get; set; }
        public decimal ShortScaleRSMDAmount { get; set; }
        public decimal TerrorismAmount { get; set; }
        public decimal TerrorismRate { get; set; }
        public decimal ShortScaleTerrorismAmount { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public FireRiskConfiguration RiskDetails { get; set; }
        //lockdown discount
        public decimal LockdownBasicDiscountAmount { get; set; }
        public decimal ShortScaleLockdownBasicDiscountAmount { get; set; }
        public decimal PremiumAfterLockdownBasicDiscount { get; set; }
        public decimal ShortScalePremiumAfterLockdownBasicDiscount { get; set; }
        public decimal LockdownRSMDTDiscountAmount { get; set; }
        public decimal ShortScaleLockdownRSMDTDiscountAmount { get; set; }
        public decimal PremiumAfterLockdownRSMDTDiscount { get; set; }
        public decimal ShortScalePremiumAfterLockdownRSMDTDiscount { get; set; }

    }
}
