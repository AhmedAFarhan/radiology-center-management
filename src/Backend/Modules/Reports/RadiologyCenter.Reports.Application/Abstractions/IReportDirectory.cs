namespace RadiologyCenter.Reports.Application.Abstractions;

/// <summary>
/// Resolves display names for patients, radiologists and examination types
/// across the Patients, ResourceManagement and Examinations modules for the report read side.
/// </summary>
public interface IReportDirectory
{
    Task<IReadOnlyDictionary<Guid, string>> ResolvePatientNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, string>> ResolveRadiologistNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, string>> ResolveExaminationTypeNamesAsync(IEnumerable<Guid> examinationIds, CancellationToken ct = default);
    Task<bool> IsExaminationCompletedAsync(Guid examinationId, CancellationToken ct = default);
}