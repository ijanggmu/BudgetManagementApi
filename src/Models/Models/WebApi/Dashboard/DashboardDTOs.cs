namespace Models.WebApi.Dashboard;

// SuperAdmin Dashboard
public record SuperAdminDashboardDto(
    int TotalTenants,
    int TotalRoles,
    int TotalLeads,
    int TotalAdmins,
    int TotalMarketingExecutives
);

// TenantAdmin Dashboard
public record TenantAdminDashboardDto(
    int TotalBranches,
    int TotalQuotations,
    int TotalLeads,
    int TotalMarketingExecutives,
    int TotalDesignations
);

// Marketing Executive Dashboard
public record MarketingExecutiveDashboardDto(
    LeadStatDto TotalLeads,
    ConversionRateDto ConversionRate,
    AveragePremiumDto AveragePremium,
    SalesFunnelDto SalesFunnel
);

public record LeadStatDto(
    int Count,
    decimal PercentageChange,
    string ChangeType // "increase" or "decrease"
);

public record ConversionRateDto(
    decimal Rate,
    decimal PercentageChange,
    string ChangeType
);

public record AveragePremiumDto(
    decimal Average,
    decimal Change,
    string ChangeType
);

public record SalesFunnelDto(
    int TotalActiveLeadsWithPremium,
    int TotalQualifiedLeadsWithPremium,
    int TotalConvertedLeadsWithPremium,
    int TotalClosedLeadsWithPremium
);

