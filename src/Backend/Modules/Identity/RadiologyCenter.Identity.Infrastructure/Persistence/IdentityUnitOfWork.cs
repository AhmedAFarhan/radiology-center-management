using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Identity.Infrastructure.Persistence;

public class IdentityUnitOfWork : UnitOfWork<IdentityDbContext>, IIdentityUnitOfWork
{
    public IdentityUnitOfWork(IdentityDbContext context, IDomainEventDispatcher events)
        : base(context, events)
    {
    }
}
