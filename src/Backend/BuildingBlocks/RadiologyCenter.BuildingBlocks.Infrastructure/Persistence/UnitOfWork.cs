using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public class UnitOfWork<TContext> : IUnitOfWork, IUnitOfWork<TContext>
    where TContext : AppDbContext
{
    private readonly TContext _context;
    private readonly IDomainEventDispatcher _events;

    public UnitOfWork(TContext context, IDomainEventDispatcher events)
    {
        _context = context;
        _events = events;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            if (_context.Database.CurrentTransaction is not null)
                return await _context.SaveChangesAsync(ct);

            // Own the transaction before the domain-event interceptor enrolls the
            // DbContext in Wolverine's outbox. Otherwise enrollment can open a
            // transaction that nobody commits and a successful request loses data.
            int count;
            await using (var transaction = await _context.Database.BeginTransactionAsync(ct))
            {
                count = await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            await _events.FlushAsync(ct);
            return count;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(MessageCodes.Shared.ConcurrencyConflict, "The record was modified by another user. Please refresh and try again.", ex);
        }
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(ct);
        return new DbUnitOfWorkTransaction(transaction, _events);
    }

}

internal sealed class DbUnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;
    private readonly IDomainEventDispatcher _events;

    public DbUnitOfWorkTransaction(
        IDbContextTransaction transaction, IDomainEventDispatcher events)
    {
        _transaction = transaction;
        _events = events;
    }

    public DbTransaction? DbTransaction => _transaction.GetDbTransaction();

    public async Task CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await _transaction.CommitAsync(ct);
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
        // SQL is committed before messages become visible to local handlers.
        // A dispatch failure must never attempt to roll back a committed transaction.
        await _events.FlushAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default) =>
        await _transaction.RollbackAsync(ct);

    public async ValueTask DisposeAsync() =>
        await _transaction.DisposeAsync();
}
