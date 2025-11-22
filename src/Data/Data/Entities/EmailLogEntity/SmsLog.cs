using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities.Log;
public class SmsLogConfiguration : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> builder)
    {
        builder.Property(e => e.SmsType)
            .HasConversion(
                v => v.ToString(),
                v => (SmsType)Enum.Parse(typeof(SmsType), v));
    }
}
public enum SmsType
{
    RegisterCustomer,
    CustomerKycApproval,
    CustomerKycRejection,
    RegisterCoorperate,
    PolicyCreation,
    PolicyRenewal,
    ClaimRegister,
    CliamApproval,
    SurveyorAppointment,
    RegisterFoDo,

}
public class SmsLog : ApplicationBaseEntity
{
    public string From { get; set; }
    public string To { get; set; }
    public string Body { get; set; }
    public DateTime SentDate { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public SmsType SmsType { get; set; }
}
