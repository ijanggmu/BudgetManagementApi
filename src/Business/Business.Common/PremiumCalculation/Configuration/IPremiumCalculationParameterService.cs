using System.Threading;
using Models.WebApi.Admin.PremiumCalculation;
using SharedKernel.Operation;

namespace Business.Common.PremiumCalculation.Configuration;

public interface IPremiumCalculationParameterService
{
    Task<Result<List<PremiumCalculationParameterDto>>> GetParametersByConfigurationIdAsync(string configurationId, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationParameterDto>> GetParameterByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationParameterDto>> CreateParameterAsync(string configurationId, CreatePremiumCalculationParameterDto dto, CancellationToken cancellationToken = default);
    Task<Result<PremiumCalculationParameterDto>> UpdateParameterAsync(string id, CreatePremiumCalculationParameterDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteParameterAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> BulkUpdateParametersAsync(string configurationId, List<CreatePremiumCalculationParameterDto> parameters, CancellationToken cancellationToken = default);
}

