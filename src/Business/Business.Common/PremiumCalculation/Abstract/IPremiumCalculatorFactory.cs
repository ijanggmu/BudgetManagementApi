namespace UnderwritingService.Calculation.PremiumCalculation.Abstract
{
    public interface IPremiumCalculatorFactory
    {
        IPremiumCalculator GetCalculator(string portfolioOrClass);
    }
}
