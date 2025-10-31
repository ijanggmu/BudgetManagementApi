using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Configuration.GlobalConfiguration;
using System.Collections.Generic;

namespace Models.Common.Policy.Configuration
{
    public class CommonConfigurationViewModel
    {
        public List<GlobalConfigurationViewModel> GlobalConfigurations { get; set; }
        public List<CalculationConfigurationViewModel> CalculationConfigurations { get; set; }
        public bool IsConfigured { get; set; }
    }
}
