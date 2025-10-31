using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Risk
{
    public class HouseholdRiskViewModel
    {       
        public List<HouseholdAsset> AssetRisk { get; set; } = new List<HouseholdAsset>();
    }

    public class HouseholdAsset
    {
        public List<RiskBaseViewModel> Owndamagepremium { get; set; } = new List<RiskBaseViewModel>();
        public List<RiskBaseViewModel> RSMDT { get; set; } = new List<RiskBaseViewModel>();
    }
}

