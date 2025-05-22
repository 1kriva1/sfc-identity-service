using System.ComponentModel.DataAnnotations.Schema;

using SFC.Identity.Domain.Common.Interfaces;

namespace SFC.Identity.Domain.Common;
public abstract class BaseEntity<TId> : IEntity
{
    public TId Id { get; set; } = default!;

    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public virtual void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public virtual void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}