using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

public class ResourceManagementUnitOfWork : UnitOfWork<ResourceManagementDbContext>, IResourceManagementUnitOfWork
{
    public ResourceManagementUnitOfWork(ResourceManagementDbContext context)
        : base(context)
    {
    }
}
