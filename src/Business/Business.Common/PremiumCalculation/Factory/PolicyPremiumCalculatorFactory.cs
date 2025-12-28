using Business.Common.PremiumCalculation.Abstract;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Factory;
public class PolicyPremiumCalculatorFactory : IPolicyPremiumCalculatorFactory
{
    private readonly Dictionary<string, IPolicyPremiumCalculator> _calculatorMap;

    public PolicyPremiumCalculatorFactory(
        IEnumerable<IPolicyPremiumCalculator> calculators)
    {
        _calculatorMap = new Dictionary<string, IPolicyPremiumCalculator>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var calculator in calculators)
        {
            foreach (var alias in calculator.SupportedPortfolioAliases)
            {
                if (_calculatorMap.ContainsKey(alias))
                {
                    throw new InvalidOperationException(
                        $"Duplicate calculator registered for portfolio alias '{alias}'");
                }

                _calculatorMap.Add(alias, calculator);
            }
        }
    }

    public IPolicyPremiumCalculator GetCalculator(string portfolioAlias)
    {
        if (!_calculatorMap.TryGetValue(portfolioAlias, out var calculator))
            throw new InvalidOperationException(
                $"No premium calculator found for portfolio alias '{portfolioAlias}'");

        return calculator;
    }
}
