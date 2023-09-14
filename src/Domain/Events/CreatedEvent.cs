namespace Domain.Events;

public class CreatedEvent<T> : DomainEvent where T : IEntity
{
    public CreatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
