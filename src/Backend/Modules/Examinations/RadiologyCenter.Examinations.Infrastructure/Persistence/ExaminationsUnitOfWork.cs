using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence;

public class ExaminationsUnitOfWork : UnitOfWork<ExaminationsDbContext>, IExaminationsUnitOfWork
{
    public ExaminationsUnitOfWork(ExaminationsDbContext context, IDomainEventDispatcher eventDispatcher)
        : base(context, eventDispatcher)
    {
    }
}
