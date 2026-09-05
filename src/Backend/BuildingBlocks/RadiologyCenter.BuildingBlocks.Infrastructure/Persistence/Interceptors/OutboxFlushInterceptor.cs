using Microsoft.EntityFrameworkCore.Diagnostics;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;

public class OutboxFlushInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _eventDispatcher;

    public OutboxFlushInterceptor(IDomainEventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken ct = default)
    {
        if (eventData.Context?.Database.CurrentTransaction is null)
        {
            await _eventDispatcher.FlushAsync(ct);
        }

        return result;
    }
}
