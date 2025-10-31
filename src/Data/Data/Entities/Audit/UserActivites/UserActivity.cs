using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Audit.UserActivites;
public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.HasIndex(x => x.UserId).IsUnique(false);
        builder.HasIndex(x => x.UserName).IsUnique(false);
        builder.HasIndex(x => x.At).IsUnique(false);
        builder.HasIndex(x => x.EndAt).IsUnique(false);
    }
}

[EntityTypeConfiguration(typeof(UserActivityConfiguration))]
public class UserActivity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string RequestSchema { get; set; }

    [MaxLength(200)]
    public string RequestHost { get; set; }

    [MaxLength(200)]
    public string RequestPath { get; set; }

    public string RequestPathAlias { get; set; }

    [MaxLength(200)]
    public string RequestMethod { get; set; }
    public string RequestQueryString { get; set; }
    public string RequestHeader{ get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }

    [MaxLength(50)]
    public string IpAddress { get; set; }

    public DateTimeOffset At { get; set; }
    public DateTimeOffset EndAt { get; set; }

    public int ResponseStatusCode { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string UserAgent { get; set; }

    public bool VisibleToUserExceptAdmin { get; set; } // flag to differentiate the logs shown to admin and customer.
    public string Module { get; set; }
    public string CorrelationId { get; set; }
    public double ResponseTime { get; set; }
}
