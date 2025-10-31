using Data.Entities.AdminEntity;
using Data.Entities.AgentEntity;
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
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options,
            IUserProfileService userProfileService) : base(options)
        {
            _userProfileService = userProfileService;
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


        #endregion  DbSets



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
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "Users"); });
            builder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "Roles"); });
            builder.Entity<ApplicationUserRoles>(entity => { entity.ToTable("UserRoles"); });
            builder.Entity<ApplicationRoleClaim>(b => { b.ToTable("RoleClaims"); });
        }
    }
}

