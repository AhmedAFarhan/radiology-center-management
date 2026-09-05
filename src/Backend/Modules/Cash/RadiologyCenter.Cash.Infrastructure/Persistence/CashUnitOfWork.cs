using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Cash.Application.Abstractions;

namespace RadiologyCenter.Cash.Infrastructure.Persistence;

public class CashUnitOfWork : UnitOfWork<CashDbContext>, ICashUnitOfWork
{
    public CashUnitOfWork(CashDbContext context)
        : base(context)
    {
    }
}