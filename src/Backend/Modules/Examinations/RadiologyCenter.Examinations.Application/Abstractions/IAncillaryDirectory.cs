namespace RadiologyCenter.Examinations.Application.Abstractions;

/// <summary>
/// Resolves display names for staff and referral doctors, and counts active machines by modality,
/// across the ResourceManagement module for the analytics read side.
/// </summary>
public interface IAncillaryDirectory
{
    Task<IReadOnlyDictionary<Guid, string>> ResolveStaffNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, string>> ResolveReferralNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, int>> GetActiveMachineCountByModalityAsync(CancellationToken ct = default);
}
