using System.Text.Json;
using Data.Entities.BaseEntity;
using Data.Entities.Common;
using Data.Entities.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Net.WebRequestMethods;

namespace Data.Entities.Identity;
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(p => p.Email).IsUnique(false);

        builder.HasIndex(p => p.PhoneNumber).IsUnique(false);

        builder.Property(e => e.UserTotpBackUpCodes)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                        v => JsonSerializer.Deserialize<List<UserTotpBackUpCode>>(v, new JsonSerializerOptions()),
                        new ValueComparer<List<UserTotpBackUpCode>>(
                            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));

        builder.HasMany(c => c.Otp)
              .WithOne(cd => cd.User)
              .HasForeignKey(cd => cd.UserId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TwoFaSetupStatus)
           .HasConversion(v => v.ToString(),
                v => (TwoFaSetupStatus)Enum.Parse(typeof(TwoFaSetupStatus), v));

    }
}

[EntityTypeConfiguration(typeof(ApplicationUserConfiguration))]
public class ApplicationUser : IdentityUser<string>, IBaseEntity, IAuditableEntity, ITenantEntity
{
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryDateTime { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsDisabled { get; set; }
    public string TotpSecurityStamp { get; set; }
    public string TotpToken { get; set; }
    public DateTime? TotpTokenEnd { get; set; }
    public int? PhoneCountryId { get; set; }
    public Country PhoneCountry { get; set; }
    public List<UserTotpBackUpCode> UserTotpBackUpCodes { get; set; }
    public ICollection<UserOtp> Otp { get; set; }
    public TwoFaSetupStatus TwoFaSetupStatus { get; set; } = TwoFaSetupStatus.NotStarted;
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// Optional department ID for HOD/users restricted to a single department (e.g. memo requests).
    /// </summary>
    public string DepartmentId { get; set; }
}

public class UserTotpBackUpCode
{
    public string CodeHash { get; set; }
    public bool IsUsed { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UsedOn { get; set; }
}
public enum TwoFaSetupStatus
{
    NotStarted = 0,
    SetupGenerated = 1,
    Verified = 2,
}

