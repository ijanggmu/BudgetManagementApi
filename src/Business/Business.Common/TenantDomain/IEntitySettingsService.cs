using Models.BeemaEdgeApi.Entity;
using Models.Common;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IEntitySettingsService
{
    Task<Result<EntitySettingsResponseDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<MessageResponseModel>> UpdateAsync(UpdateEntitySettingsDto dto, CancellationToken cancellationToken = default);
}




