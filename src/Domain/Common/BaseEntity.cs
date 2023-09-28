using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Common;

public abstract class BaseEntity: IEntity<long>
{
    [Description("唯一标识")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public virtual long Id { get; set; }
    /// <summary>
    /// 乐观并发标记
    /// </summary>
    [Description("乐观并发标记")]
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    private readonly List<DomainEvent> _domainEvents = new();

    [NotMapped]
    [JsonIgnore]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
