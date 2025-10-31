using System.Collections.Generic;

namespace Models.Common.Policy.Configuration.GlobalConfiguration
{
    public class GlobalSetupConfigurationViewModel
    {
        public GlobalConfigurationViewModel GlobalConfigurationViewModel { get; set; }
        public List<GlobalConfigurationViewModel> ContinuousGlobalConfigurations { get; set; }
        public List<GlobalConfigurationViewModel> DiscreteGlobalConfigurations { get; set; }
        public List<GlobalConfigurationViewModel> MultiDiscreteGlobalConfigurations { get; set; }
        public List<GlobalConfigurationViewModel> GlobalConfigurations { get; set; }
    }
}
