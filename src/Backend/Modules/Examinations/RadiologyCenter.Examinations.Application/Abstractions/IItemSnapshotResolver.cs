namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IItemSnapshotResolver
{
    Task<IReadOnlyDictionary<Guid, decimal>> ResolveAsync(IEnumerable<Guid> itemIds, CancellationToken ct);
}
