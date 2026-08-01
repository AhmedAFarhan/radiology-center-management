using RadiologyCenter.Examinations.Domain.ValueObjects;

namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IItemSnapshotResolver
{
    Task<IReadOnlyDictionary<Guid, ItemSnapshot>> ResolveAsync(IEnumerable<Guid> itemIds, CancellationToken ct);
}
