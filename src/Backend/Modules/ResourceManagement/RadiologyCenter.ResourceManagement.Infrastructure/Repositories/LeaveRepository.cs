using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Repositories;

public class LeaveRepository : BaseRepository<Leave, Guid>, ILeaveRepository
{
    public LeaveRepository(ResourceManagementDbContext context) : base(context) { }
}
