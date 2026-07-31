using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Domain.Entities;
using RadiologyCenter.Patients.Infrastructure.Persistence;

namespace RadiologyCenter.Patients.Infrastructure.Repositories;

public class PatientRepository : BaseRepository<Patient, Guid>, IPatientRepository
{
    public PatientRepository(PatientsDbContext context) : base(context) { }
}
