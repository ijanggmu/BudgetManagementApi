using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.SystemEnum.Otp;

namespace Data.Entities.Identity;
public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
{
    public void Configure(EntityTypeBuilder<UserOtp> builder)
    {

        builder.Property(x => x.Type)
            .HasConversion(v => v.ToString(),
                 v => (OtpType)Enum.Parse(typeof(OtpType), v));

        builder.Property(x => x.Channel)
            .HasConversion(v => v.ToString(),
                 v => (OtpChannel)Enum.Parse(typeof(OtpChannel), v));

        builder.Property(x => x.Module)
            .HasConversion(v => v.ToString(),
                 v => (SystemModule)Enum.Parse(typeof(SystemModule), v));

        builder.HasOne(x => x.User)
           .WithMany()
           .HasForeignKey(x => x.UserId)
           .IsRequired();
    }
}

[EntityTypeConfiguration(typeof(UserOtpConfiguration))]
public class UserOtp : ApplicationBaseEntity
{
    public UserOtp()
    {

    }
    public UserOtp(string userId, string otpCode, OtpType type, OtpChannel channel, SystemModule module, DateTime otpSentDateTime)
    {
        UserId = userId;
        OTPCode = otpCode;
        Type = type;
        Channel = channel;
        OTPSentDateTime = otpSentDateTime;
        OTPToken = Guid.NewGuid().ToString();
        IsUsed = false;
        Module = module;
    }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string OTPCode { get; set; }
    public OtpType Type { get; set; }
    public OtpChannel Channel { get; set; }
    public SystemModule Module { get; set; }
    public DateTime OTPSentDateTime { get; set; }

    public bool IsUsed { get; set; }
    public string OTPToken { get; set; }
}
