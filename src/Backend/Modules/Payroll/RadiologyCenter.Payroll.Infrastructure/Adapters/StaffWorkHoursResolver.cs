using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Adapters;

public class StaffWorkHoursResolver : IStaffWorkHoursResolver
{
    private readonly IWorkShiftRepository _workShiftRepository;

    public StaffWorkHoursResolver(IWorkShiftRepository workShiftRepository) => _workShiftRepository = workShiftRepository;

    public async Task<decimal> GetWorkedHoursAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<WorkShift>();
        spec.AddCriteria(w => w.StaffId == staffId);
        spec.AddCriteria(w => w.Date.Date >= from.Date && w.Date.Date <= to.Date);

        var shifts = await _workShiftRepository.FindAsync(spec, ct);

        decimal hours = 0;
        foreach (var shift in shifts)
            hours += (decimal)(shift.EndTime - shift.StartTime).TotalHours;

        return Math.Round(hours, 2);
    }
}
