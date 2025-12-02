using Business.Common.PremiumCalculation.Calculator;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace UnderwritingService.Calculation.PremiumCalculation.Factory
{
    public class PremiumCalculatorFactory : IPremiumCalculatorFactory
    {
        private readonly ConfigurablePremiumCalculator _configurableCalculator;

        public PremiumCalculatorFactory(ConfigurablePremiumCalculator configurableCalculator)
        {
            _configurableCalculator = configurableCalculator;
        }

        public IPremiumCalculator GetCalculator(string portfolioOrClass)
        {
            // Return the new configurable calculator for all portfolios
            // All calculation logic is now driven by database configuration
            return _configurableCalculator;
        }
    }
}
