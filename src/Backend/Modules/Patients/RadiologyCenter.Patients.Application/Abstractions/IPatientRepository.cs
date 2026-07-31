using RadiologyCenter.Patients.Domain.Entities;

namespace RadiologyCenter.Patients.Application.Abstractions;

public interface IPatientRepository : IBaseRepository<Patient, Guid>;
