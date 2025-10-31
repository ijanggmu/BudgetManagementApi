using Models.Common.Policy.Configuration.CalculationConfiguration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Models.Common.Policy.Policy.Marine
{
    public class MarineConfigurationsViewModel
    {
        public List<CalculationConfigurationViewModel> InlandTransitConfiguration { get; set; }
        public List<SelectListItem> CurrencyNames { get; set; }
    }
}
