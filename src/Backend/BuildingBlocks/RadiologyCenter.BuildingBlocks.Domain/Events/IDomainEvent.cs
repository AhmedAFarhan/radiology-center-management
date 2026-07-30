namespace RadiologyCenter.BuildingBlocks.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
