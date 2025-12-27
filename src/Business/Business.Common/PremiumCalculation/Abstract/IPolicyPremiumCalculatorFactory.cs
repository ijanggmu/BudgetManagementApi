using Models.Common.Policy.Policy;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Abstract;
public interface IPolicyPremiumCalculatorFactory
{
    IPolicyPremiumCalculator GetCalculator(string portfolioOrClass);
    //IPolicyPremiumCalculator GetCalculator(InsuranceType insuranceType);
}
