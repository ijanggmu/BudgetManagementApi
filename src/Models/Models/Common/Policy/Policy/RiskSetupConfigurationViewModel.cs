using Models.Common.Policy.Configuration.CalculationConfiguration;
using System.Collections.Generic;

namespace Models.Common.Policy.Policy
{
    public class RiskSetupConfigurationViewModel
    {
        public CalculationConfigurationViewModel CalculationConfigurationViewModel { get; set; }
        public List<CalculationConfigurationViewModel> ContinuousCalculationConfigurations { get; set; }
        public List<CalculationConfigurationViewModel> DiscreteCalculationConfigurations { get; set; }
        public List<CalculationConfigurationViewModel> MultiDiscreteCalculationConfigurations { get; set; }
        public List<CalculationConfigurationViewModel> MultilevelContinuousCalculationConfigurations { get; set; }
        public List<CalculationConfigurationViewModel> BooleanCalculationConfigurations { get; set; }
        
        public List<CalculationConfigurationViewModel> CalculationConfigurations { get; set; }
    }
}
