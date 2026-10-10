namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface ITransactionLock
{
    Task AcquireAsync(IUnitOfWorkTransaction transaction, IEnumerable<string> resources, CancellationToken ct = default);
}
