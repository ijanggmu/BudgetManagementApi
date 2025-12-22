using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WebApi.TenantDTOs;

public record CreateLeadPublicDto(string FullName, string Email, string? Phone, string ProductCode);
public record LeadActivityDto(string Kind, string Notes);
public record RatingPreviewDto(string ProductCode, DateOnly AsOf, IDictionary<string, object> Inputs);
public record RatingPreviewResultDto(decimal Total, object Breakdown, int RateTable, int Formula);

public record CreateQuotationDto(Guid ProductId, Guid ProspectId, IEnumerable<QuotationItemDto> Items);
public record UpdateQuotationDto(string? Status, decimal? TotalPremium, decimal? DiscountPercent, DateOnly? ValidUntil, IEnumerable<QuotationItemDto>? Items);
public record QuotationItemDto(Guid CoverageId, decimal SumInsured);
public record ComputeQuotationDto(DateOnly AsOf, IDictionary<string, object> Inputs);

// Tenant DTOs
public record CreateTenantDto(string Name, string Slug, CreateCompanyBrandingDto CompanyBranding, CreateTenantAdminDto AdminUser, bool IsActive = true, int ThemeVersion = 1);
public record CreateTenantAdminDto(string Email, string Username, string FullName, string Password);
public record CreateCompanyBrandingDto(string LogoUrl = default!,    // CDN/blob URL
     string PaletteJson = "{}",    // AA contrast enforced
     string TypographyJson = "{}",
     int Version = 1);

public record UpdateTenantDto(string? Name, string? Slug, bool? IsActive, int? ThemeVersion);
public record TenantsResponseDto(string Id, string Name, string Slug, bool IsActive, int ThemeVersion, DateTime CreatedOn);
public record TenantResponseDto(
    string Id, 
    string Name, 
    string Slug, 
    bool IsActive, 
    int ThemeVersion, 
    DateTime CreatedOn,
    BrandingResponseDto? Branding = null);

// Tenant Login DTOs
public record TenantLoginRequestDto(string Slug, string Username, string Password);
public record TenantLoginResponseDto(
    string AccessToken, 
    int AccessTokenExpiryInSeconds, 
    string RefreshToken, 
    int RefreshTokenExpiryInSeconds, 
    string TenantId, 
    string TenantName,
    BrandingResponseDto? Branding = null);

// Tenant Dropdown DTO
public record TenantDropdownDto(string Id, string Name, string Slug);

// Branding DTOs
public record BrandingResponseDto(
    string TenantId,
    string LogoUrl,
    string PaletteJson,
    string TypographyJson,
    int Version,
    DateTime CreatedOn
);

public record UpdateBrandingDto(
    string? LogoUrl,
    string? PaletteJson,
    string? TypographyJson
);

// Lead Response DTOs
public record LeadResponseDto(
    string Id,
    string ProspectId,
    string Status,
    string Source,
    Guid? OwnerUserId,
    DateTime CreatedOn,
    ProspectResponseDto? Prospect
);

public record ProspectResponseDto(
    string Id,
    string PrimaryContactId,
    ContactResponseDto? PrimaryContact
);

public record ContactResponseDto(
    string Id,
    string FullName,
    string Email,
    string Phone
);

public record LeadActivityResponseDto(
    string Id,
    string LeadId,
    string Kind,
    string Notes,
    DateTimeOffset When
);

// Quotation Response DTOs
public record QuotationResponseDto(
    string Id,
    string Number,
    string Status,
    string ProductId,
    string ProspectId,
    decimal? TotalPremium,
    decimal? DiscountPercent,
    DateOnly? ValidUntil,
    string? PdfUrl,
    DateTime CreatedOn,
    List<QuotationItemResponseDto> Items
);

public record QuotationItemResponseDto(
    string Id,
    string QuotationId,
    string CoverageId,
    decimal SumInsured,
    decimal Premium
);

// User Response DTOs
public record UserResponseDto(
    string Id,
    string UserName,
    string? Email,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    bool IsDisabled,
    string? TenantId
);

//public static class LeadStateMachine
//{
//    private static readonly Dictionary<LeadStatus, LeadStatus[]> _map = new()
//    {
//        [LeadStatus.New] = new[] { LeadStatus.Qualified, LeadStatus.Lost },
//        [LeadStatus.Qualified] = new[] { LeadStatus.Contacted, LeadStatus.Lost },
//        [LeadStatus.Contacted] = new[] { LeadStatus.Quoted, LeadStatus.Lost },
//        [LeadStatus.Quoted] = new[] { LeadStatus.Won, LeadStatus.Lost },
//    };
//    public static bool CanTransition(LeadStatus from, LeadStatus to) => _map.TryGetValue(from, out var next) && next.Contains(to);
//}
