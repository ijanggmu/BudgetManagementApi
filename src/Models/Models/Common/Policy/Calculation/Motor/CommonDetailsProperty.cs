namespace Models.Common.Policy.Calculation
{
    public class CommonDetailsProperty
    {
        public CalculationSubDetailAmountModel MinimumBasicPremium { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel CalculatedOwnDamagePremium { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel BasicPremium { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PerSeatCapacity { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel Trailor { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountOwnDamage { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountTPL { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountRSMDT { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountRSMD { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountPA { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountPADriver { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountPAPassenger { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LayupDiscountPAHelper { get; set; } = new CalculationSubDetailAmountModel();
        public int ValueOfTrailorEntered { get; set; }
        public CalculationSubDetailAmountModel TrailorMinAmount { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel TPLAsPerCC { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel LoadingForOldVehicle { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PrivateHire { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel VoluntaryExcess { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel NCD { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel OwnUseDiscount { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel TPLNCD { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel DirectDiscount { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel DirectDiscountForPADriver { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel DirectDiscountForPAPassenger { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel DirectDiscountForPAHelper { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel RecoveryCharge { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel RecoveryChargeBeforeLayup { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PAForPaidDriver { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PAForPassenger { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PAForHelper { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel RiotAndStrikeMD { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel Terrorism { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel DriverRSMDT { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PassengerRSMDT { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel HelperRSMDT { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel MinimumRSMDTAmount { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel RSMDtoPillionRider { get; set; } = new CalculationSubDetailAmountModel();
        //remove below properties
        public CalculationSubDetailAmountModel BasicProRataOrShortScale { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel ThirdPartyProRataOrShortScale { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel RSMDTProRataOrShortScale { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel PersonalAccidentProRataOrShortScale { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel TotalBasicPremium { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel SpecialDiscount { get; set; } = new CalculationSubDetailAmountModel();
        public CalculationSubDetailAmountModel SpecialDiscountTPL { get; set; } = new CalculationSubDetailAmountModel();

        // public decimal MinimumBasicPremium { get; set; }
        //  public decimal MinimumRSMDTAmount { get; set; }
        public decimal SelectedVoluntaryExcess { get; set; }
        public decimal SumInsured { get; set; }

        public string AgeDifference { get; set; }
        public decimal RSMDTSumInsuredForDriver { get; set; }

        public decimal RSMDTSumInsuredForPassengers { get; set; }

        public int Days { get; set; }
        public bool IsProRateOrShortScaleAmount { get; set; }

        public decimal ShortScaleRate { get; set; }
        public decimal BasicShortScaleRate { get; set; }
        public decimal TPLShortScaleRate { get; set; }
        public decimal PAShortScaleRate { get; set; }
        public decimal RSMDTShortScaleRate { get; set; }
    }
}