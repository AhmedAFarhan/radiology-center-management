using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Patients.Application.Abstractions;

namespace RadiologyCenter.Patients.Infrastructure.Persistence;

public class PatientsUnitOfWork : UnitOfWork<PatientsDbContext>, IPatientsUnitOfWork
{
    public PatientsUnitOfWork(PatientsDbContext context)
        : base(context)
    {
    }
}
