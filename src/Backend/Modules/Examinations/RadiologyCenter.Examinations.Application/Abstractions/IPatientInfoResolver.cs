namespace RadiologyCenter.Examinations.Application.Abstractions;

public sealed record PatientInfo(Guid Id, string FullName, string PatientCode);

public interface IPatientInfoResolver
{
    Task<PatientInfo?> ResolveAsync(Guid patientId, CancellationToken ct = default);
}
