using Data.Entities.BaseEntity;
using Data.Entities.PolicyE2e;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.SystemEnum.Payment;

namespace Data.Entities.Payment;
public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {

        builder.Property(x => x.Status)
            .HasConversion(v => v.ToString(),
                 v => (PaymentTransactionStatus)Enum.Parse(typeof(PaymentTransactionStatus), v));

        builder.Property(x => x.PaymentGateway)
         .HasConversion(v => v.ToString(),
              v => (PaymentGateway)Enum.Parse(typeof(PaymentGateway), v));
    }
}

[EntityTypeConfiguration(typeof(PaymentTransactionConfiguration))]
public class PaymentTransaction : ApplicationBaseEntity
{
    public string PolicyPurchaseId { get; set; }
    public PolicyDraft PolicyDraft { get; set; }
    public string GatewayTransactionId { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? VerifiedAt { get; set; }
    public string RawResponse { get; set; } = default!;
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Initiated;
    public string PaymentGatewayStatus { get; set; }
    public PaymentGateway PaymentGateway { get; set; }
}
