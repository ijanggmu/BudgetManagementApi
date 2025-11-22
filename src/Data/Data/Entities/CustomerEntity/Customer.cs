using Data.Entities.BaseEntity;
using Data.Entities.Identity;
using Data.Entities.Payment;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Data.Entities.AdminEntity;

namespace Data.Entities.CustomerEntity;
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {

        builder.Property(x => x.CoreSyncStatus)
            .HasConversion(v => v.ToString(),
                 v => (CoreSyncStatus)Enum.Parse(typeof(CoreSyncStatus), v));
        builder.Property(x => x.KycStatus)
           .HasConversion(v => v.ToString(),
                v => (KYCStatus)Enum.Parse(typeof(KYCStatus), v));
        builder.HasIndex(x => x.PanNo).IsUnique();

        builder.HasOne(c => c.KycReviewedByUser)
                        .WithMany()
                        .HasForeignKey(c => c.KycReviewedByUserId)
                        .OnDelete(DeleteBehavior.Restrict);

    }
}

[EntityTypeConfiguration(typeof(CustomerConfiguration))]
public class Customer : ApplicationBaseEntity, ITenantEntity
{
    public string FullName { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }
    public string IndividualType { get; set; }
    public string CourtesyTitle { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullNameNepali { get; set; }
    public string Gender { get; set; }
    public string PanNo { get; set; }
    public string MaritalStatus { get; set; }
    public string DobBS { get; set; }
    public string DobAD { get; set; }
    public string NIDNumber { get; set; }
    public string CitizenshipNo { get; set; }
    public string CitizenshipIssueDistrict { get; set; }
    public string CitizenshipIssueDate { get; set; }
    public string PassportNumber { get; set; }
    public string PassportIssueDate { get; set; }
    public string PassportExpiryDate { get; set; }
    public string PassportIssuePlace { get; set; }
    public string VoterIdNumber { get; set; }
    public string LicenseNumber { get; set; }
    public string LicenseIssueDate { get; set; }
    public string LicenseExpiryDate { get; set; }
    public string Occupation { get; set; }
    public string UserPhoto { get; set; }
    public string IdFrontPhoto { get; set; }
    public string IdBackPhoto { get; set; }
    public string DigitalSignature { get; set; }
    public CoreSyncStatus CoreSyncStatus { get; set; } = CoreSyncStatus.NotSynced;
    public KYCStatus KycStatus { get; set; } = KYCStatus.NotSubmitted;
    public string KycRejectedReason { get; set; }
    public DateTime? KycReviewedAt { get; set; }
    public string KycReviewedByUserId { get; set; }
    public virtual ApplicationUser KycReviewedByUser { get; set; }
    public string GrandFatherName { get; set; }
    public string FatherName { get; set; }
    public string MotherName { get; set; }
    public string PartyID { get; set; }
    public string PartyCode { get; set; }
    public string DIANumber { get; set; }
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = [];

}
public enum KYCStatus
{
    NotSubmitted = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
public enum CoreSyncStatus
{
    NotSynced = 0,
    Pending = 1,
    Succeeded = 2,
    Failed = 3,
    Retrying = 4
}

