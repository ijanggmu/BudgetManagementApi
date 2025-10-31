using Microsoft.EntityFrameworkCore;

namespace Data.Entities.Audit;

public class EntityAudit
{
    public Guid Id { get; set; }
    public string TableName { get; set; }
    public string PrimaryKey { get; set; }
    public DateTimeOffset At { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public string AffectedColumns { get; set; }
    public EntityState State { get; set; }
}
