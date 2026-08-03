using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Infrastructure.Persistence;

namespace RadiologyCenter.Payroll.Infrastructure.Repositories;

public class SalaryComponentRepository : BaseRepository<SalaryComponent, Guid>, ISalaryComponentRepository
{
    public SalaryComponentRepository(PayrollDbContext context) : base(context)
    {
    }
}