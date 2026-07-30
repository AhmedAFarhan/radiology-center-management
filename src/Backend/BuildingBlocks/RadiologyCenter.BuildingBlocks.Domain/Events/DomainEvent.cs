namespace RadiologyCenter.BuildingBlocks.Domain.Events;

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
