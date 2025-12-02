using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationConfigurationService
{
    Task<Result<PremiumCalculationConfigurationDto>> GetConfigurationAsync(string portfolioAlias, string fiscalYear, DateTime? effectiveDate = null);
    Task<Result<PremiumCalculationConfigurationDto>> GetConfigurationByIdAsync(string id);
    Task<Result<List<PremiumCalculationConfigurationDto>>> GetAllConfigurationsAsync(string? portfolioAlias = null, string? fiscalYear = null);
    Task<Result<PremiumCalculationConfigurationDto>> CreateConfigurationAsync(CreatePremiumCalculationConfigurationDto dto);
    Task<Result<PremiumCalculationConfigurationDto>> UpdateConfigurationAsync(string id, UpdatePremiumCalculationConfigurationDto dto);
    Task<Result<bool>> DeleteConfigurationAsync(string id);
    Task<Result<PremiumCalculationConfigurationDto>> ActivateConfigurationAsync(string id, string fiscalYear);
    Task<Result<PremiumCalculationConfigurationDto>> CloneConfigurationAsync(string id, string newFiscalYear);
    Task<Result<object>> GetParameterValueAsync(string portfolioAlias, string fiscalYear, string parameterKey);
    Task<Result<bool>> ValidateConfigurationAsync(string configurationId);
}

