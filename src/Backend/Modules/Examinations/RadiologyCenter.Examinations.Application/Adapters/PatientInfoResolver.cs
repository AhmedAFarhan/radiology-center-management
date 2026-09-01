using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Patients.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Adapters;

public sealed class PatientInfoResolver : IPatientInfoResolver
{
    private readonly IPatientRepository _patientRepository;

    public PatientInfoResolver(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientInfo?> ResolveAsync(Guid patientId, CancellationToken ct = default)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId, ct);
        return patient is null ? null : new PatientInfo(patient.Id, patient.FullName, patient.PatientCode);
    }
}
