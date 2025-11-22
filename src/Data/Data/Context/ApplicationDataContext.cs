using Data.Entities.AdminEntity;
using Data.Entities.AgentEntity;
using Data.Entities.BaseEntity;
using Data.Entities.Common;
using Data.Entities.CorporateEntity;
using Data.Entities.CustomerEntity;
using Data.Entities.Draft;
using Data.Entities.EmailLogEntity;
using Data.Entities.Identity;
using Data.Entities.ITIEntity;
using Data.Entities.Log;
using Data.Entities.MotorEntity;
using Data.Entities.Payment;
using Data.Entities.PrivateVehicleEntity;
using Data.Extensions;
using Data.Entities.Tenant;
using SharedKernel.Models.Tenancy;
using Data.Infrastructure;
using Microsoft.Extensions.Options;
using SharedKernel.Config;
using SharedKernel.Constant.Roles;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDataContext : IdentityDbContext<ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>, ApplicationUserRoles, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ITenantContext _tenantContext;
        
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options,
            IUserProfileService userProfileService,
            ITenantContext tenantContext,
            ITenantSchemaProvider tenantSchemaProvider,
            IOptions<TenancyOptions> tenancyOptions) : base(options)
        {
            _userProfileService = userProfileService;
            _tenantContext = tenantContext;
            _tenantSchemaProvider = tenantSchemaProvider;
            _tenancyOptions = tenancyOptions.Value;
        }

        #region DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Corporate> Corporates { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<UserOtp> UserOtps { get; set; }
        public DbSet<PolicyDraft> PolicyDrafts { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Motor> Motors { get; set; }
        public DbSet<PrivateVehicle> PrivateVehicles { get; set; }
        public DbSet<InternationalTravelInsurance> ITI { get; set; }
        public DbSet<CustomerAddress> Addresses { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<SmsLog> SmsLogs { get; set; }
        public DbSet<ITIFamilyMember> ITIFamilyMembers { get; set; }

        // Multitenant
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<CompanyBranding> CompanyBrandings { get; set; }
        public DbSet<AttendanceEntry> AttendanceEntries { get; set; }
        public DbSet<RenewalReminder> RenewalReminders { get; set; }
        public DbSet<NotificationHistory> NotificationHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadActivity> LeadActivities { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Prospect> Prospects { get; set; }


        #endregion  DbSets



        private readonly ITenantSchemaProvider _tenantSchemaProvider;
        private readonly TenancyOptions _tenancyOptions;

        public string? CurrentTenantId => _tenantContext?.TenantId;

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
            
            // Set TenantId for new entities implementing ITenantEntity
            // Skip SuperAdmin users - they should have null TenantId
            if (_tenantContext?.TenantId is string tid)
            {
                var addedTenantEntities = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added && e.Entity is ITenantEntity)
                    .Select(e => new { Entity = (ITenantEntity)e.Entity, Entry = e });
                
                foreach (var item in addedTenantEntities)
                {
                    // Skip setting TenantId for SuperAdmin users
                    if (item.Entity is ApplicationUser user && !string.IsNullOrEmpty(user.Id))
                    {
                        try
                        {
                            // Query roles directly from database to avoid circular dependency
                            var isSuperAdmin = Set<ApplicationUserRoles>()
                                .Where(ur => ur.UserId == user.Id && !ur.IsDeleted)
                                .Join(Set<ApplicationRole>()
                                    .Where(r => !r.IsDeleted),
                                    ur => ur.RoleId,
                                    r => r.Id,
                                    (ur, r) => r.Name)
                                .Contains(SystemRoles.SuperAdmin);
                            
                            if (isSuperAdmin)
                            {
                                // SuperAdmin should have null TenantId to access all tenants
                                continue;
                            }
                        }
                        catch
                        {
                            // If role check fails, proceed with normal TenantId assignment
                        }
                    }
                    
                    // Only set TenantId if it's null or empty
                    if (string.IsNullOrEmpty(item.Entity.TenantId))
                    {
                        item.Entity.TenantId = tid;
                    }
                }
                
                // Also update modified entities if TenantId is empty (for backward compatibility)
                var modifiedTenantEntities = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified && e.Entity is ITenantEntity)
                    .Select(e => new { Entity = (ITenantEntity)e.Entity, Entry = e });
                
                foreach (var item in modifiedTenantEntities)
                {
                    // Skip setting TenantId for SuperAdmin users
                    if (item.Entity is ApplicationUser user && !string.IsNullOrEmpty(user.Id))
                    {
                        try
                        {
                            // Query roles directly from database to avoid circular dependency
                            var isSuperAdmin = Set<ApplicationUserRoles>()
                                .Where(ur => ur.UserId == user.Id && !ur.IsDeleted)
                                .Join(Set<ApplicationRole>()
                                    .Where(r => !r.IsDeleted),
                                    ur => ur.RoleId,
                                    r => r.Id,
                                    (ur, r) => r.Name)
                                .Contains(SystemRoles.SuperAdmin);
                            
                            if (isSuperAdmin)
                            {
                                // SuperAdmin should have null TenantId to access all tenants
                                // Also clear TenantId if it was previously set
                                if (!string.IsNullOrEmpty(item.Entity.TenantId))
                                {
                                    item.Entity.TenantId = null;
                                }
                                continue;
                            }
                        }
                        catch
                        {
                            // If role check fails, proceed with normal TenantId assignment
                        }
                    }
                    
                    if (string.IsNullOrEmpty(item.Entity.TenantId))
                    {
                        item.Entity.TenantId = tid;
                    }
                }
            }
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
            builder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "Roles"); });
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
                .WithOne()
                .HasForeignKey<CompanyBranding>(x => x.TenantId)
                .HasPrincipalKey<Tenant>(x => x.Id);

            builder.Entity<CompanyBranding>()
                .HasKey(x => x.TenantId);

            // Global query filter by TenantId for tenanted entities
            foreach (var entityType in builder.Model.GetEntityTypes().Where(t => typeof(TenantEntity).IsAssignableFrom(t.ClrType)))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var tenantIdProp = System.Linq.Expressions.Expression.Property(parameter, nameof(TenantEntity.TenantId));
                var ctxTenantIdProp = System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this), nameof(CurrentTenantId));
                var left = System.Linq.Expressions.Expression.Convert(tenantIdProp, typeof(string));
                var body = System.Linq.Expressions.Expression.Equal(left, ctxTenantIdProp);
                var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}

