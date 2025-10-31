namespace Data.Entities.Audit;

public class SaveChangesAudit
{
    public Guid Id { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public bool Succeeded { get; set; }
    public string ErrorMessage { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string RemoteIpAddress { get; set; }
    public ICollection<EntityAudit> Entities { get; } = new List<EntityAudit>();
}
