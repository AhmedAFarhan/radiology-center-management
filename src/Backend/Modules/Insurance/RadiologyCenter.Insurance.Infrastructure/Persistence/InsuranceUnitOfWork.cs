using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Insurance.Application.Abstractions;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence;

public class InsuranceUnitOfWork : UnitOfWork<InsuranceDbContext>, IInsuranceUnitOfWork
{
    public InsuranceUnitOfWork(InsuranceDbContext context)
        : base(context)
    {
    }
}