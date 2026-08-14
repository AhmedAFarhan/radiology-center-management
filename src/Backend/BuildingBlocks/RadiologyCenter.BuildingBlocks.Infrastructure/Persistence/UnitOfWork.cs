using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;

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
        try
        {
            await DispatchDomainEventsAsync(ct);
            var result = await _context.SaveChangesAsync(ct);

            if (_context.Database.CurrentTransaction is null)
                await _eventDispatcher.FlushAsync(ct);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("The record was modified by another user. Please refresh and try again.", ex);
        }
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(ct);
        return new DbUnitOfWorkTransaction(transaction, this, _eventDispatcher);
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
            await _eventDispatcher.DispatchAsync(entity, _context, ct);
        }
    }
}

internal sealed class DbUnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public DbUnitOfWorkTransaction(
        IDbContextTransaction transaction,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher)
    {
        _transaction = transaction;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
    }

    public DbTransaction? DbTransaction => _transaction.GetDbTransaction();

    public async Task CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
            await _eventDispatcher.FlushAsync(ct);
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
