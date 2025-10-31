namespace Models.Common.Policy.Risk
{
    public class MotorRiskViewModel
    {
        public List<RiskBaseViewModel> Owndamagepremium { get; set; } = new List<RiskBaseViewModel>();
        public List<RiskBaseViewModel> DirectDiscount { get; set; } = new List<RiskBaseViewModel>();
        public List<RiskBaseViewModel> ThirdPary { get; set; } = new List<RiskBaseViewModel>();
        public List<RiskBaseViewModel> RSMDT { get; set; } = new List<RiskBaseViewModel>();
        public CommonStringViewModel OwnDamageProRataShortScale { get; set; } = new CommonStringViewModel();
        public CommonStringViewModel ThirdPartyProRataShortScale { get; set; } = new CommonStringViewModel();
        public CommonStringViewModel RSMDTProRataShortScale { get; set; } = new CommonStringViewModel();
    }
    public class CommonStringViewModel
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public int? IntValue1 { get; set; }
        public int? IntValue2 { get; set; }
        public int? IntValue3 { get; set; }
        public bool BoolValue { get; set; }
        public DateTime? DateValue1 { get; set; }
        public DateTime? DateValue2 { get; set; }
        public string Value6 { get; set; }
        public decimal DecimalValue { get; set; }
        public decimal DecimalValue2 { get; set; }
    }

}
