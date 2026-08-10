namespace RadiologyCenter.Cash.Application.Abstractions;

/// <summary>
/// Resolves display names for users referenced by cash sessions and handovers.
/// </summary>
public interface ICashDirectory
{
    Task<IReadOnlyDictionary<Guid, string>> ResolveUserNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}