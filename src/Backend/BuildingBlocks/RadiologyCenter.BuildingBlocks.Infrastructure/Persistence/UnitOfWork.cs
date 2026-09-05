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

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(MessageCodes.Shared.ConcurrencyConflict, "The record was modified by another user. Please refresh and try again.", ex);
        }
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(ct);
        return new DbUnitOfWorkTransaction(transaction);
    }

}

internal sealed class DbUnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;

    public DbUnitOfWorkTransaction(
        IDbContextTransaction transaction)
    {
        _transaction = transaction;
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
    }

    public async Task RollbackAsync(CancellationToken ct = default) =>
        await _transaction.RollbackAsync(ct);

    public async ValueTask DisposeAsync() =>
        await _transaction.DisposeAsync();
}
