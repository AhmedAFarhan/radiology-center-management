using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Infrastructure.Persistence;

namespace RadiologyCenter.Payroll.Infrastructure.Repositories;

public class SalaryRepository : BaseRepository<Salary, Guid>, ISalaryRepository
{
    public SalaryRepository(PayrollDbContext context) : base(context)
    {
    }
}