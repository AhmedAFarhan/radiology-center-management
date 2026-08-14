using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Catalog.Application.Abstractions;

namespace RadiologyCenter.Catalog.Infrastructure.Persistence;

public class CatalogUnitOfWork : UnitOfWork<CatalogDbContext>, ICatalogUnitOfWork
{
    public CatalogUnitOfWork(CatalogDbContext context, IDomainEventDispatcher eventDispatcher)
        : base(context, eventDispatcher)
    {
    }
}