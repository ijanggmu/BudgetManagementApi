
using System.Collections.Generic;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class GPAConfigurationViewModel
    {
        public List<ClassificationRiskCoverageRateViewModel> RiskCoverageViewModels { get; set; }
        public decimal ELimit { get; set; }
    }
}
