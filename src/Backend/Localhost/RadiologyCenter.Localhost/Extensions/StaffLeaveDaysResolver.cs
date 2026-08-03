using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.Localhost.Extensions;

public class StaffLeaveDaysResolver : IStaffLeaveResolver
{
    private readonly ILeaveRepository _leaveRepository;

    public StaffLeaveDaysResolver(ILeaveRepository leaveRepository) => _leaveRepository = leaveRepository;

    public async Task<int> GetUnpaidLeaveDaysAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<Leave>(l => l.LeaveType == LeaveType.Unpaid);
        spec.AddCriteria(l => l.StaffId == staffId);
        spec.AddCriteria(l => l.StartDate <= to);
        spec.AddCriteria(l => l.EndDate >= from);

        var leaves = await _leaveRepository.FindAsync(spec, ct);

        int days = 0;
        foreach (var leave in leaves)
        {
            var overlapStart = leave.StartDate.Date > from.Date ? leave.StartDate.Date : from.Date;
            var overlapEnd = leave.EndDate.Date < to.Date ? leave.EndDate.Date : to.Date;

            if (overlapStart <= overlapEnd)
                days += (overlapEnd - overlapStart).Days + 1;
        }

        return days;
    }
}