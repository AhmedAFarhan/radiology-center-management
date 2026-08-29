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
            var result = await _context.SaveChangesAsync(ct);

            if (_context.Database.CurrentTransaction is null)
                await _eventDispatcher.FlushAsync(ct);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(
                MessageCodes.Shared.ConcurrencyConflict,
                "The record was modified by another user. Please refresh and try again.",
                ex);
        }
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(ct);
        return new DbUnitOfWorkTransaction(transaction, this, _eventDispatcher);
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
            await _transaction.CommitAsync(ct);
            await _eventDispatcher.FlushAsync(ct);
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
