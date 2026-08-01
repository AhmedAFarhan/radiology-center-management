using System.Data.Common;

namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    DbTransaction? DbTransaction { get; }
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
