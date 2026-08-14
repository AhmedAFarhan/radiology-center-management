using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IAggregateRoot aggregate, DbContext context, CancellationToken ct = default);
    Task FlushAsync(CancellationToken ct = default);
}
