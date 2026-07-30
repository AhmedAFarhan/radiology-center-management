using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IAggregateRoot aggregate, CancellationToken ct = default);
}
