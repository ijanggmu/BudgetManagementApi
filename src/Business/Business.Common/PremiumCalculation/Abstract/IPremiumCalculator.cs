using Models.Common.Policy.Calculation;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Policy;
using System.Threading.Tasks;

namespace UnderwritingService.Calculation.PremiumCalculation.Abstract
{
    public interface IPremiumCalculator
    {
        Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model);
        Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation);
    }public interface IPolicyPremiumCalculator
    {
        IReadOnlyCollection<string> SupportedPortfolioAliases { get; }

        Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model);
        Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation);
    }
}
