using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using Wolverine;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMessageBus _bus;

    public DomainEventDispatcher(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task DispatchAsync(IAggregateRoot aggregate, CancellationToken ct = default)
    {
        var events = aggregate.DomainEvents.ToArray();
        aggregate.ClearDomainEvents();

        foreach (var domainEvent in events)
        {
            await _bus.PublishAsync(domainEvent);
        }
    }
}
