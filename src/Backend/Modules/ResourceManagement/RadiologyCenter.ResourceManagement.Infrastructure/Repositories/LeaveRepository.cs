using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Repositories;

public class LeaveRepository : BaseRepository<Leave, Guid>, ILeaveRepository
{
    public LeaveRepository(ResourceManagementDbContext context) : base(context) { }

    public async Task<bool> HasOverlapAsync(
        Guid staffId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeLeaveId = null,
        CancellationToken ct = default)
        => await DbSet.AnyAsync(
            l => l.StaffId == staffId
                && l.StartDate <= endDate
                && l.EndDate >= startDate
                && (!excludeLeaveId.HasValue || l.Id != excludeLeaveId.Value),
            ct);
}
