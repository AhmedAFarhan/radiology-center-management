using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RadiologyCenter.Insurance.Application.Abstractions;

/// <summary>
/// Resolves display names for patients and examination types across the
/// Patients and Examinations modules for the insurance read side.
/// </summary>
public interface IInsuranceDirectory
{
    Task<IReadOnlyDictionary<Guid, string>> ResolvePatientNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, string>> ResolveExaminationTypeNamesAsync(IEnumerable<Guid> examinationIds, CancellationToken ct = default);
}