using Models.Common;
using Models.WebApi.Gateway;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Gateway;

public interface IGatewayConfigurationService
{
    // Email Gateway
    Task<Result<List<EmailGatewayResponseDto>>> GetAllEmailGatewaysAsync(CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default);
    Task<Result<EmailGatewayResponseDto>> GetEmailGatewayByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<EmailGatewayResponseDto>> CreateEmailGatewayAsync(CreateEmailGatewayDto dto, CancellationToken cancellationToken = default);
    Task<Result<EmailGatewayResponseDto>> UpdateEmailGatewayAsync(string id, UpdateEmailGatewayDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteEmailGatewayAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<EmailGatewayResponseDto>> GetActiveEmailGatewayAsync(CancellationToken cancellationToken = default);

    // SMS Gateway
    Task<Result<List<SmsGatewayResponseDto>>> GetAllSmsGatewaysAsync(CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default);
    Task<Result<SmsGatewayResponseDto>> GetSmsGatewayByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<SmsGatewayResponseDto>> CreateSmsGatewayAsync(CreateSmsGatewayDto dto, CancellationToken cancellationToken = default);
    Task<Result<SmsGatewayResponseDto>> UpdateSmsGatewayAsync(string id, UpdateSmsGatewayDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteSmsGatewayAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<SmsGatewayResponseDto>> GetActiveSmsGatewayAsync(CancellationToken cancellationToken = default);
}

