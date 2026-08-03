using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Infrastructure.Persistence;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence;

public class PayrollUnitOfWork : UnitOfWork<PayrollDbContext>, IPayrollUnitOfWork
{
    public PayrollUnitOfWork(PayrollDbContext context, IDomainEventDispatcher eventDispatcher)
        : base(context, eventDispatcher)
    {
    }
}