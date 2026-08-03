using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Repositories;

public class StaffRepository : BaseRepository<Staff, Guid>, IStaffRepository
{
    public StaffRepository(ResourceManagementDbContext context) : base(context) { }
}
