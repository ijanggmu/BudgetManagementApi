using Models.Common.Policy.Policy.Fire;

namespace Business.Common.PremiumCalculation.Service;
public interface IPropertyRiskConfigurationService
{
    Task CreatePropertyRiskConfiguration(FireRiskConfiguration model);
    Task<List<FireRiskConfiguration>> GetAllPropertyRiskConfig();
    Task<FireRiskConfiguration> GetSinglePropertyRiskConfigurationById(string id);
    Task<FireRiskConfiguration> GetSinglePropertyRiskConfigurationByRiskCode(string riskCode);
    Task ImportPropertyRiskConfiguration(List<FireRiskConfiguration> data);
    Task UpdatePropertyRiskConfiguration(FireRiskConfiguration model);
}