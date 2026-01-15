using Data.Entities.AdminEntity;
using Data.Entities.BaseEntity;
using Data.Entities.Common;
using Data.Entities.CustomerEntity;
using Data.Entities.EmailLogEntity;
using Data.Entities.Identity;
using Data.Entities.Log;
using Data.Entities.Tenant;
using Data.Extensions;
using Data.Infrastructure;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.Config;
using SharedKernel.Models.Tenancy;

namespace Data.Context;

public class ApplicationDataContext(DbContextOptions<ApplicationDataContext> options,
    IUserProfileService userProfileService,
    ITenantContext tenantContext,
    ITenantSchemaProvider tenantSchemaProvider,
    IOptions<TenancyOptions> tenancyOptions) : IdentityDbContext<ApplicationUser,
    ApplicationRole,
    string,
    IdentityUserClaim<string>, ApplicationUserRoles, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>(options)
{
    private readonly IUserProfileService _userProfileService = userProfileService;
    private readonly ITenantContext _tenantContext = tenantContext;

    #region DbSets
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<UserOtp> UserOtps { get; set; }
    public DbSet<CustomerAddress> Addresses { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }
    public DbSet<SmsLog> SmsLogs { get; set; }

    // Multitenant
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<CompanyBranding> CompanyBrandings { get; set; }
    public DbSet<AttendanceEntry> AttendanceEntries { get; set; }
    public DbSet<NotificationHistory> NotificationHistories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Noticeboard> Noticeboards { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Prospect> Prospects { get; set; }


    // Premium Calculation
    public DbSet<CurrencyExchangeRateConfiguration> CurrencyExchangeRateConfigurations { get; set; }
    public DbSet<TravelUSDRate> TravelUSDRates { get; set; }
    public DbSet<HEOMITravelRate> HEOMITravelRates { get; set; }

    // Branch and Designation
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Designation> Designations { get; set; }

    // Gateway Configurations
    public DbSet<EmailGatewayConfiguration> EmailGatewayConfigurations { get; set; }
    public DbSet<SmsGatewayConfiguration> SmsGatewayConfigurations { get; set; }

    #endregion  DbSets



    private readonly ITenantSchemaProvider _tenantSchemaProvider = tenantSchemaProvider;
    private readonly TenancyOptions _tenancyOptions = tenancyOptions.Value;

    public string CurrentTenantId => _tenantContext?.TenantId;

    /// <summary>
    /// Checks if the entity type implements IBaseEntity (which includes IsDeleted).
    /// </summary>
    private static bool ImplementsIBaseEntity(Type entityType)
    {
        return typeof(IBaseEntity).IsAssignableFrom(entityType);
    }

    /// <summary>
    /// Creates an expression for the IsDeleted filter: e.IsDeleted == false
    /// </summary>
    private static System.Linq.Expressions.Expression CreateIsDeletedFilterExpression(
        System.Linq.Expressions.ParameterExpression parameter)
    {
        var isDeletedProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(IBaseEntity.IsDeleted));
        var falseConstant = System.Linq.Expressions.Expression.Constant(false);
        return System.Linq.Expressions.Expression.Equal(isDeletedProperty, falseConstant);
    }

    /// <summary>
    /// Creates an expression for the TenantId filter: e.TenantId == CurrentTenantId
    /// </summary>
    private System.Linq.Expressions.Expression CreateTenantIdFilterExpression(
        System.Linq.Expressions.ParameterExpression parameter, Type entityType)
    {
        // Use reflection to get the TenantId property - this is only done once during model creation
        var tenantIdProperty = entityType.GetProperty(nameof(ITenantEntity.TenantId));
        if (tenantIdProperty == null)
            throw new InvalidOperationException($"Entity type {entityType.Name} implements ITenantEntity but does not have TenantId property.");

        var tenantIdProp = System.Linq.Expressions.Expression.Property(parameter, tenantIdProperty);
        var ctxTenantIdProp = System.Linq.Expressions.Expression.Property(
            System.Linq.Expressions.Expression.Constant(this), nameof(CurrentTenantId));
        var tenantIdLeft = System.Linq.Expressions.Expression.Convert(tenantIdProp, typeof(string));
        return System.Linq.Expressions.Expression.Equal(tenantIdLeft, ctxTenantIdProp);
    }

    /// <summary>
    /// Applies global query filters to an entity type.
    /// </summary>
    private void ApplyGlobalQueryFilters(ModelBuilder builder, Type entityType, bool hasTenantId, bool hasIsDeleted)
    {
        if (!hasIsDeleted && !hasTenantId)
            return;

        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        System.Linq.Expressions.Expression filterExpression = null;

        // Add TenantId filter if applicable
        if (hasTenantId)
        {
            filterExpression = CreateTenantIdFilterExpression(parameter, entityType);
        }

        // Add IsDeleted filter if applicable
        if (hasIsDeleted)
        {
            var isDeletedFilter = CreateIsDeletedFilterExpression(parameter);
            filterExpression = filterExpression == null
                ? isDeletedFilter
                : System.Linq.Expressions.Expression.AndAlso(filterExpression, isDeletedFilter);
        }

        if (filterExpression != null)
        {
            var lambda = System.Linq.Expressions.Expression.Lambda(filterExpression, parameter);
            builder.Entity(entityType).HasQueryFilter(lambda);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateShadowFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateShadowFields();
        return base.SaveChanges();
    }

    private void UpdateShadowFields()
    {
        var userId = _userProfileService.GetUserId();
        ChangeTracker.SetAuditableEntityPropertyValues(userId);

        // Set TenantId automatically for entities implementing ITenantEntity
        // Note: SuperAdmin users should have TenantId set to null explicitly when creating users
        // This avoids expensive database queries during SaveChanges
        ChangeTracker.SetTenantIdPropertyValues(_tenantContext?.TenantId);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // set per-tenant default schema when configured
        if (_tenancyOptions.Strategy == TenantStorageStrategy.SeparateSchema)
        {
            var schema = _tenantSchemaProvider.GetSchemaOrNull();
            if (!string.IsNullOrWhiteSpace(schema))
            {
                builder.HasDefaultSchema(schema);
            }
        }
        foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "Users"); });

        // Configure ApplicationRole: Remove unique constraint on NormalizedName and add composite unique index
        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable(name: "Roles");

            // Remove the default unique index on NormalizedName (from Identity framework)
            // The base OnModelCreating creates a unique index on NormalizedName with name "RoleNameIndex"
            // We need to remove it and create a composite unique index instead
            entity.HasIndex(r => r.NormalizedName)
                .HasDatabaseName("RoleNameIndex")
                .IsUnique(false); // Remove unique constraint

            // Add composite unique index on NormalizedName and TenantId
            // This allows the same role name to exist for different tenants
            // Note: TenantId can be null for global roles (SuperAdmin), so null values are treated as distinct
            entity.HasIndex(r => new { r.NormalizedName, r.TenantId })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false") // Only enforce uniqueness on non-deleted roles
                .HasDatabaseName("IX_Roles_NormalizedName_TenantId");
        });

        builder.Entity<ApplicationUserRoles>(entity => { entity.ToTable("UserRoles"); });
        builder.Entity<ApplicationRoleClaim>(b => { b.ToTable("RoleClaims"); });

        // Concurrency and relationships for tenanted entities
        foreach (var entityType in builder.Model.GetEntityTypes().Where(t => typeof(TenantEntity).IsAssignableFrom(t.ClrType)))
        {
            var clr = entityType.ClrType;
            builder.Entity(clr).Property<byte[]>(nameof(TenantEntity.RowVersion)).IsRowVersion();
        }

        //builder.Entity<Tenant>()
        //    .HasAlternateKey(x => x.Id);

        builder.Entity<Tenant>()
            .HasOne(x => x.Branding)
            .WithOne(x => x.Tenant)
            .HasForeignKey<CompanyBranding>(x => x.TenantId)
            .HasPrincipalKey<Tenant>(x => x.Id);

        builder.Entity<CompanyBranding>()
            .HasKey(x => x.TenantId);


        // Notification indexes for performance
        builder.Entity<Notification>(entity =>
        {
            entity.HasIndex(n => new { n.UserId, n.ReadAt })
                .HasDatabaseName("IX_Notifications_UserId_ReadAt");
            entity.HasIndex(n => n.SentAt)
                .HasDatabaseName("IX_Notifications_SentAt");
            entity.HasIndex(n => n.TenantId)
                .HasDatabaseName("IX_Notifications_TenantId");
        });

        // Noticeboard indexes for performance
        builder.Entity<Noticeboard>(entity =>
        {
            entity.HasIndex(n => n.TenantId)
                .HasDatabaseName("IX_Noticeboards_TenantId");
            entity.HasIndex(n => new { n.IsActive, n.IsPinned, n.Priority })
                .HasDatabaseName("IX_Noticeboards_Active_Pinned_Priority");
            entity.HasIndex(n => n.CreatedOn)
                .HasDatabaseName("IX_Noticeboards_CreatedOn");
        });

        // Apply global query filters for IsDeleted and TenantId
        // This is done in a single pass for better performance
        var entityTypes = builder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();

        // Entities that should NOT have global query filters applied
        var excludedFromGlobalFilters = new[]
        {
            typeof(ApplicationUserRoles)
        };

        foreach (var entityType in entityTypes)
        {
            // Skip entities that should not have global query filters
            if (excludedFromGlobalFilters.Contains(entityType))
                continue;

            var implementsITenantEntity = typeof(ITenantEntity).IsAssignableFrom(entityType);
            var implementsIBaseEntity = ImplementsIBaseEntity(entityType);

            // Skip if entity doesn't need any filters
            if (!implementsITenantEntity && !implementsIBaseEntity)
                continue;

            ApplyGlobalQueryFilters(
                builder,
                entityType,
                hasTenantId: implementsITenantEntity,
                hasIsDeleted: implementsIBaseEntity);
        }
    }
}

