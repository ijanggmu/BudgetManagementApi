using Data.Entities.BaseEntity;
using Data.Entities.CustomerEntity;
using Data.Entities.ITIEntity;
using Data.Entities.MotorEntity;
using Data.Entities.Payment;
using Data.Entities.PrivateVehicleEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Common.Policy.Policy;
using SharedKernel.SystemEnum.Payment;

namespace Data.Entities.PolicyE2e;

public class PolicyDraftConfiguration : IEntityTypeConfiguration<PolicyDraft>
{
    public void Configure(EntityTypeBuilder<PolicyDraft> builder)
    {

        builder.Property(x => x.Status)
            .HasConversion(v => v.ToString(),
                 v => (PurchaseStatus)Enum.Parse(typeof(PurchaseStatus), v));

        builder.Property(x => x.PaymentGateway)
                             .HasConversion(
                                 v => v != null ? v.ToString() : null,
                                 v => v != null ? Enum.Parse<PaymentGateway>(v) : null
                             );

        builder.Property(x => x.InsuranceType)
                            .HasConversion(
                                v => v != null ? v.ToString() : null,
                                v => v != null ? Enum.Parse<InsuranceType>(v) : null
                            );

    }
}

[EntityTypeConfiguration(typeof(PolicyDraftConfiguration))]
public class PolicyDraft : ApplicationBaseEntity
{
    public InsuranceType? InsuranceType { get; set; }
    public string DraftNo { get; set; }
    public string PaymentTransactionId { get; set; }
    public PaymentTransaction PaymentTransaction { get; set; }
    public decimal NetPremium { get; set; }
    public string BranchCode { get; set; }
    public string PortfolioAlias { get; set; }
    public string PortfolioId { get; set; }
    public string PolicyNumber { get; set; }
    public string DocumentNumber { get; set; }
    public string InvoiceNumber { get; set; }
    public string ReceiptNumber { get; set; }
    public string TypeOfParty { get; set; }
    public string PortfolioParent { get; set; }
    public string Class { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string BancassuanceBankName { get; set; }
    public string BancassuanceBankBranch { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string MotorId { get; set; }
    public Motor Motor { get; set; }
    public string ITIId { get; set; }
    public InternationalTravelInsurance ITI { get; set; }
    public string PrivateVehicleId { get; set; }
    public PrivateVehicle PrivateVehicle { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public PurchaseStatus Status { get; set; }
    public PaymentGateway? PaymentGateway { get; set; }
    public string TransactionReference { get; set; } = default!;
    public bool IsSubmitted { get; set; }
    public decimal SumInsured { get; set; }
    public decimal BasicPremium { get; set; }
    public decimal ThirdPartyPremium { get; set; }
    public decimal RSMDTPremium { get; set; }
    public decimal PersonalAccidentPremium { get; set; }
    public decimal GrossPremium { get; set; }
    public decimal TotalPremium { get; set; }
    public decimal StampDuty { get; set; }
    public decimal VatAmount { get; set; }
    public decimal GovernmentSubsidyAmount { get; set; }
    public decimal PayableAmount { get; set; }
}
