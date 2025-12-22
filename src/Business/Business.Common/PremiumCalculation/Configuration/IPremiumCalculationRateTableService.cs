using System.Threading;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationRateTableService
{
    Task<Result<List<PremiumCalculationRateTableDto>>> GetRateTablesByConfigurationIdAsync(string configurationId, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationRateTableDto>> GetRateTableByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationRateTableDto>> CreateRateTableAsync(string configurationId, CreatePremiumCalculationRateTableDto dto, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationRateTableDto>> UpdateRateTableAsync(string id, CreatePremiumCalculationRateTableDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteRateTableAsync(string id, CancellationToken cancellationToken = default);
}

