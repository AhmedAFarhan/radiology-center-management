using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public class UnitOfWork<TContext> : IUnitOfWork, IUnitOfWork<TContext>
    where TContext : AppDbContext
{
    private readonly TContext _context;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public UnitOfWork(TContext context, IDomainEventDispatcher eventDispatcher)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await _context.SaveChangesAsync(ct);
        await DispatchDomainEventsAsync(ct);
        return result;
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(ct);
        return new DbUnitOfWorkTransaction(transaction, this);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        var entries = _context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToArray();

        foreach (var entity in entries)
        {
            await _eventDispatcher.DispatchAsync(entity, ct);
        }
    }
}

internal sealed class DbUnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;
    private readonly IUnitOfWork _unitOfWork;

    public DbUnitOfWorkTransaction(IDbContextTransaction transaction, IUnitOfWork unitOfWork)
    {
        _transaction = transaction;
        _unitOfWork = unitOfWork;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
            await _transaction.CommitAsync(ct);
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default) =>
        await _transaction.RollbackAsync(ct);

    public async ValueTask DisposeAsync() =>
        await _transaction.DisposeAsync();
}
