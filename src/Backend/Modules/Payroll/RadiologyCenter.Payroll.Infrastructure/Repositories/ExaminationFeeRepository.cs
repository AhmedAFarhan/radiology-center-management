using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Infrastructure.Persistence;

namespace RadiologyCenter.Payroll.Infrastructure.Repositories;

public class ExaminationFeeRepository : BaseRepository<ExaminationFee, Guid>, IExaminationFeeRepository
{
    public ExaminationFeeRepository(PayrollDbContext context) : base(context) { }
}