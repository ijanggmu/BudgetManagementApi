using System.Threading;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationConfigurationService
{
    Task<Result<PremiumCalculationConfigurationDto>> GetConfigurationAsync(string portfolioAlias, string fiscalYear, DateTime? effectiveDate = null, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationConfigurationDto>> GetConfigurationByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<PremiumCalculationConfigurationDto>>> GetAllConfigurationsAsync(string? portfolioAlias = null, string? fiscalYear = null, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationConfigurationDto>> CreateConfigurationAsync(CreatePremiumCalculationConfigurationDto dto, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationConfigurationDto>> UpdateConfigurationAsync(string id, UpdatePremiumCalculationConfigurationDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteConfigurationAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationConfigurationDto>> ActivateConfigurationAsync(string id, string fiscalYear, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationConfigurationDto>> CloneConfigurationAsync(string id, string newFiscalYear, CancellationToken cancellationToken = default);
    Task<Result<object>> GetParameterValueAsync(string portfolioAlias, string fiscalYear, string parameterKey, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateConfigurationAsync(string configurationId, CancellationToken cancellationToken = default);
}

