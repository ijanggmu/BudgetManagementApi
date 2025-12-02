using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationRateTableService
{
    Task<Result<List<PremiumCalculationRateTableDto>>> GetRateTablesByConfigurationIdAsync(string configurationId);
    Task<Result<PremiumCalculationRateTableDto>> GetRateTableByIdAsync(string id);
    Task<Result<PremiumCalculationRateTableDto>> CreateRateTableAsync(string configurationId, CreatePremiumCalculationRateTableDto dto);
    Task<Result<PremiumCalculationRateTableDto>> UpdateRateTableAsync(string id, CreatePremiumCalculationRateTableDto dto);
    Task<Result<bool>> DeleteRateTableAsync(string id);
}

