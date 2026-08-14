using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Application.Abstractions;

public interface ILeaveRepository : IBaseRepository<Leave, Guid>
{
    Task<bool> HasOverlapAsync(Guid staffId, DateTime startDate, DateTime endDate, Guid? excludeLeaveId = null, CancellationToken ct = default);
}
