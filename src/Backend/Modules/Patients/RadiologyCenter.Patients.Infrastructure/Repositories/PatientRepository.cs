using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Domain.Entities;
using RadiologyCenter.Patients.Infrastructure.Persistence;

namespace RadiologyCenter.Patients.Infrastructure.Repositories;

public class PatientRepository : BaseRepository<Patient, Guid>, IPatientRepository
{
    public PatientRepository(PatientsDbContext context) : base(context) { }

    public async Task<Patient?> GetByPatientCodeAsync(string patientCode, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(p => p.PatientCode == patientCode, ct);

    public async Task<bool> ExistsByPatientCodeAsync(string patientCode, CancellationToken ct = default) =>
        await DbSet.AsNoTracking().AnyAsync(p => p.PatientCode == patientCode, ct);
}
