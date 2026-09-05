using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using Wolverine.EntityFrameworkCore;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IDbContextOutbox _outbox;

    public DomainEventDispatcher(IDbContextOutbox outbox)
    {
        _outbox = outbox;
    }

    public async Task DispatchAsync(IAggregateRoot aggregate, DbContext context, CancellationToken ct = default)
    {
        _outbox.Enroll(context);

        var events = aggregate.DomainEvents.ToArray();
        aggregate.ClearDomainEvents();

        foreach (var domainEvent in events) await _outbox.PublishAsync(domainEvent);
    }

    public async Task FlushAsync(CancellationToken ct = default) => await _outbox.FlushOutgoingMessagesAsync();
}
