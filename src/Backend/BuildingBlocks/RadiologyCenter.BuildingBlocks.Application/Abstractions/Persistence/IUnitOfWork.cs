namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default);
}

public interface IUnitOfWork<TContext>
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
