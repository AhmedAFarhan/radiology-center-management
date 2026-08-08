using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Reports.Application.Abstractions;

namespace RadiologyCenter.Reports.Infrastructure.Persistence;

public class ReportsUnitOfWork : UnitOfWork<ReportsDbContext>, IReportsUnitOfWork
{
    public ReportsUnitOfWork(ReportsDbContext context, IDomainEventDispatcher eventDispatcher)
        : base(context, eventDispatcher)
    {
    }
}