using Business.Common.StringCipher;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.Gateway;
using SharedKernel.Models.Tenancy;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Gateway;

public class GatewayConfigurationService : IGatewayConfigurationService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;
    private readonly ITenantContext _tenantContext;
    private readonly IUserProfileService _userProfileService;
    private readonly StringCipherService _stringCipherService;
    private readonly ILogger<GatewayConfigurationService> _logger;

    public GatewayConfigurationService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        ITenantContext tenantContext,
        IUserProfileService userProfileService,
        StringCipherService stringCipherService,
        ILogger<GatewayConfigurationService> logger)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _tenantContext = tenantContext;
        _userProfileService = userProfileService;
        _stringCipherService = stringCipherService;
        _logger = logger;
    }

    #region Email Gateway

    public async Task<Result<List<EmailGatewayResponseDto>>> GetAllEmailGatewaysAsync(
        CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.EmailGatewayConfigurations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn);

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var gateways = await result.Select(x => MapToEmailGatewayResponse(x)).ToListAsync(cancellationToken);

            var pagination = new Pagination
            {
                TotalPages = totalPage,
                CurrentPage = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
                TotalItems = totalCount,
            };

            return Result<List<EmailGatewayResponseDto>>.Success(gateways, pagination);
        }

        var allGateways = await query.Select(x => MapToEmailGatewayResponse(x)).ToListAsync(cancellationToken);
        return Result<List<EmailGatewayResponseDto>>.Success(allGateways);
    }

    public async Task<Result<EmailGatewayResponseDto>> GetEmailGatewayByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.EmailGatewayConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (gateway == null)
            return Result<EmailGatewayResponseDto>.Failed("Email gateway configuration not found.");

        return Result<EmailGatewayResponseDto>.Success(MapToEmailGatewayResponse(gateway));
    }

    public async Task<Result<EmailGatewayResponseDto>> CreateEmailGatewayAsync(
        CreateEmailGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_tenantContext.TenantId))
            return Result<EmailGatewayResponseDto>.Failed("Tenant ID is required.");

        // Check if there's already an active gateway (optional: allow multiple)
        // For now, we'll allow multiple configurations

        var userId = _userProfileService.GetUserId();

        // Encrypt sensitive data
        var encryptedUserName = await _stringCipherService.EncryptAsync(dto.UserName);
        var encryptedPassword = await _stringCipherService.EncryptAsync(dto.Password);

        var gateway = new EmailGatewayConfiguration
        {
            TenantId = _tenantContext.TenantId,
            ProviderName = dto.ProviderName,
            Host = dto.Host,
            Port = dto.Port,
            UserName = encryptedUserName,
            Password = encryptedPassword,
            FromEmail = dto.FromEmail,
            DisplayName = dto.DisplayName,
            EnableSsl = dto.EnableSsl,
            IsActive = dto.IsActive,
            AdditionalSettings = dto.AdditionalSettings,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };

        await _db.EmailGatewayConfigurations.AddAsync(gateway, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email gateway configuration created: {GatewayId} for tenant {TenantId}", gateway.Id, gateway.TenantId);

        return Result<EmailGatewayResponseDto>.Success(MapToEmailGatewayResponse(gateway));
    }

    public async Task<Result<EmailGatewayResponseDto>> UpdateEmailGatewayAsync(
        string id,
        UpdateEmailGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.EmailGatewayConfigurations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (gateway == null)
            return Result<EmailGatewayResponseDto>.Failed("Email gateway configuration not found.");

        var userId = _userProfileService.GetUserId();

        if (!string.IsNullOrWhiteSpace(dto.ProviderName))
            gateway.ProviderName = dto.ProviderName;

        if (!string.IsNullOrWhiteSpace(dto.Host))
            gateway.Host = dto.Host;

        if (dto.Port.HasValue)
            gateway.Port = dto.Port.Value;

        if (!string.IsNullOrWhiteSpace(dto.UserName))
            gateway.UserName = await _stringCipherService.EncryptAsync(dto.UserName);

        if (!string.IsNullOrWhiteSpace(dto.Password))
            gateway.Password = await _stringCipherService.EncryptAsync(dto.Password);

        if (!string.IsNullOrWhiteSpace(dto.FromEmail))
            gateway.FromEmail = dto.FromEmail;

        if (!string.IsNullOrWhiteSpace(dto.DisplayName))
            gateway.DisplayName = dto.DisplayName;

        if (dto.EnableSsl.HasValue)
            gateway.EnableSsl = dto.EnableSsl.Value;

        if (dto.IsActive.HasValue)
            gateway.IsActive = dto.IsActive.Value;

        if (dto.AdditionalSettings != null)
            gateway.AdditionalSettings = dto.AdditionalSettings;

        gateway.LastModifiedBy = userId;
        gateway.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email gateway configuration updated: {GatewayId}", gateway.Id);

        return Result<EmailGatewayResponseDto>.Success(MapToEmailGatewayResponse(gateway));
    }

    public async Task<Result<bool>> DeleteEmailGatewayAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.EmailGatewayConfigurations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (gateway == null)
            return Result<bool>.Failed("Email gateway configuration not found.");

        gateway.IsDeleted = true;
        gateway.LastModifiedBy = _userProfileService.GetUserId();
        gateway.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email gateway configuration deleted: {GatewayId}", gateway.Id);

        return Result<bool>.Success(true);
    }

    public async Task<Result<EmailGatewayResponseDto>> GetActiveEmailGatewayAsync(
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.EmailGatewayConfigurations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedOn)
            .FirstOrDefaultAsync(cancellationToken);

        if (gateway == null)
            return Result<EmailGatewayResponseDto>.Failed("No active email gateway configuration found.");

        return Result<EmailGatewayResponseDto>.Success(MapToEmailGatewayResponse(gateway));
    }

    private static EmailGatewayResponseDto MapToEmailGatewayResponse(EmailGatewayConfiguration gateway)
    {
        return new EmailGatewayResponseDto(
            gateway.Id,
            gateway.TenantId,
            gateway.ProviderName,
            gateway.Host,
            gateway.Port,
            gateway.FromEmail,
            gateway.DisplayName,
            gateway.EnableSsl,
            gateway.IsActive,
            gateway.AdditionalSettings,
            gateway.CreatedOn,
            gateway.LastModifiedOn
        );
    }

    #endregion

    #region SMS Gateway

    public async Task<Result<List<SmsGatewayResponseDto>>> GetAllSmsGatewaysAsync(
        CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.SmsGatewayConfigurations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn);

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var gateways = await result.Select(x => MapToSmsGatewayResponse(x)).ToListAsync(cancellationToken);

            var pagination = new Pagination
            {
                TotalPages = totalPage,
                CurrentPage = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
                TotalItems = totalCount,
            };

            return Result<List<SmsGatewayResponseDto>>.Success(gateways, pagination);
        }

        var allGateways = await query.Select(x => MapToSmsGatewayResponse(x)).ToListAsync(cancellationToken);
        return Result<List<SmsGatewayResponseDto>>.Success(allGateways);
    }

    public async Task<Result<SmsGatewayResponseDto>> GetSmsGatewayByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.SmsGatewayConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (gateway == null)
            return Result<SmsGatewayResponseDto>.Failed("SMS gateway configuration not found.");

        return Result<SmsGatewayResponseDto>.Success(MapToSmsGatewayResponse(gateway));
    }

    public async Task<Result<SmsGatewayResponseDto>> CreateSmsGatewayAsync(
        CreateSmsGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_tenantContext.TenantId))
            return Result<SmsGatewayResponseDto>.Failed("Tenant ID is required.");

        var userId = _userProfileService.GetUserId();

        // Encrypt sensitive data
        var encryptedApiKey = await _stringCipherService.EncryptAsync(dto.ApiKey);
        var encryptedApiSecret = !string.IsNullOrWhiteSpace(dto.ApiSecret)
            ? await _stringCipherService.EncryptAsync(dto.ApiSecret)
            : null;

        var gateway = new SmsGatewayConfiguration
        {
            TenantId = _tenantContext.TenantId,
            ProviderName = dto.ProviderName,
            ApiUrl = dto.ApiUrl,
            ApiKey = encryptedApiKey,
            ApiSecret = encryptedApiSecret,
            From = dto.From,
            IsActive = dto.IsActive,
            AdditionalSettings = dto.AdditionalSettings,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };

        await _db.SmsGatewayConfigurations.AddAsync(gateway, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("SMS gateway configuration created: {GatewayId} for tenant {TenantId}", gateway.Id, gateway.TenantId);

        return Result<SmsGatewayResponseDto>.Success(MapToSmsGatewayResponse(gateway));
    }

    public async Task<Result<SmsGatewayResponseDto>> UpdateSmsGatewayAsync(
        string id,
        UpdateSmsGatewayDto dto,
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.SmsGatewayConfigurations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (gateway == null)
            return Result<SmsGatewayResponseDto>.Failed("SMS gateway configuration not found.");

        var userId = _userProfileService.GetUserId();

        if (!string.IsNullOrWhiteSpace(dto.ProviderName))
            gateway.ProviderName = dto.ProviderName;

        if (!string.IsNullOrWhiteSpace(dto.ApiUrl))
            gateway.ApiUrl = dto.ApiUrl;

        if (!string.IsNullOrWhiteSpace(dto.ApiKey))
            gateway.ApiKey = await _stringCipherService.EncryptAsync(dto.ApiKey);

        if (dto.ApiSecret != null)
            gateway.ApiSecret = !string.IsNullOrWhiteSpace(dto.ApiSecret)
                ? await _stringCipherService.EncryptAsync(dto.ApiSecret)
                : null;

        if (!string.IsNullOrWhiteSpace(dto.From))
            gateway.From = dto.From;

        if (dto.IsActive.HasValue)
            gateway.IsActive = dto.IsActive.Value;

        if (dto.AdditionalSettings != null)
            gateway.AdditionalSettings = dto.AdditionalSettings;

        gateway.LastModifiedBy = userId;
        gateway.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("SMS gateway configuration updated: {GatewayId}", gateway.Id);

        return Result<SmsGatewayResponseDto>.Success(MapToSmsGatewayResponse(gateway));
    }

    public async Task<Result<bool>> DeleteSmsGatewayAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.SmsGatewayConfigurations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (gateway == null)
            return Result<bool>.Failed("SMS gateway configuration not found.");

        gateway.IsDeleted = true;
        gateway.LastModifiedBy = _userProfileService.GetUserId();
        gateway.LastModifiedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("SMS gateway configuration deleted: {GatewayId}", gateway.Id);

        return Result<bool>.Success(true);
    }

    public async Task<Result<SmsGatewayResponseDto>> GetActiveSmsGatewayAsync(
        CancellationToken cancellationToken = default)
    {
        var gateway = await _db.SmsGatewayConfigurations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedOn)
            .FirstOrDefaultAsync(cancellationToken);

        if (gateway == null)
            return Result<SmsGatewayResponseDto>.Failed("No active SMS gateway configuration found.");

        return Result<SmsGatewayResponseDto>.Success(MapToSmsGatewayResponse(gateway));
    }

    private static SmsGatewayResponseDto MapToSmsGatewayResponse(SmsGatewayConfiguration gateway)
    {
        return new SmsGatewayResponseDto(
            gateway.Id,
            gateway.TenantId,
            gateway.ProviderName,
            gateway.ApiUrl,
            gateway.From,
            gateway.IsActive,
            gateway.AdditionalSettings,
            gateway.CreatedOn,
            gateway.LastModifiedOn
        );
    }

    #endregion
}

