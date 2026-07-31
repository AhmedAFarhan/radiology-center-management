using RadiologyCenter.Patients.Domain.Entities;

namespace RadiologyCenter.Patients.Application.Abstractions;

public interface IPatientRepository : IBaseRepository<Patient, Guid>
{
    Task<Patient?> GetByPatientCodeAsync(string patientCode, CancellationToken ct = default);
    Task<bool> ExistsByPatientCodeAsync(string patientCode, CancellationToken ct = default);
}
