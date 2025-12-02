using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationParameterService
{
    Task<Result<List<PremiumCalculationParameterDto>>> GetParametersByConfigurationIdAsync(string configurationId);
    Task<Result<PremiumCalculationParameterDto>> GetParameterByIdAsync(string id);
    Task<Result<PremiumCalculationParameterDto>> CreateParameterAsync(string configurationId, CreatePremiumCalculationParameterDto dto);
    Task<Result<PremiumCalculationParameterDto>> UpdateParameterAsync(string id, CreatePremiumCalculationParameterDto dto);
    Task<Result<bool>> DeleteParameterAsync(string id);
    Task<Result<bool>> BulkUpdateParametersAsync(string configurationId, List<CreatePremiumCalculationParameterDto> parameters);
}

