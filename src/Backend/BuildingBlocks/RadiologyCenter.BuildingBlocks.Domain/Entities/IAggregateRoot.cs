using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.BuildingBlocks.Domain.Entities;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
