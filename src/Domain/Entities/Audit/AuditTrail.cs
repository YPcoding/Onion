namespace Domain.Entities.Audit;

public class AuditTrail : IEntity<long>
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public virtual User? Owner { get; set; }
    public AuditType AuditType { get; set; }
    public string? TableName { get; set; }
    public DateTime DateTime { get; set; }
    public Dictionary<string, object?>? OldValues { get; set; }
    public Dictionary<string, object?>? NewValues { get; set; }
    public List<string>? AffectedColumns { get; set; }
    public Dictionary<string, object> PrimaryKey { get; set; } = new();

    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public bool HasTemporaryProperties => TemporaryProperties.Any();
}
