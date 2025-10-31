using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Risk
{
    public class ElectricMotorcycleRiskViewModel
    {
        public List<RiskBaseViewModel> Owndamagepremium { get; set; } = new List<RiskBaseViewModel>();
        public List<RiskBaseViewModel> ThirdPary { get; set; } = new List<RiskBaseViewModel>();
        public List<RiskBaseViewModel> RSMDT { get; set; } = new List<RiskBaseViewModel>();
    }
}
