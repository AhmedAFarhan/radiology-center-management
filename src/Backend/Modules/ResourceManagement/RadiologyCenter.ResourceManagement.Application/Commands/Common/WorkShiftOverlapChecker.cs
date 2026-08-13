using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

internal static class WorkShiftOverlapChecker
{
    public static async Task<(bool IsConflict, string Resource)> FindConflictAsync(
        IWorkShiftRepository workShiftRepository,
        Guid staffId,
        Guid? equipmentId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludingId,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<WorkShift>();
        spec.AddCriteria(w => w.Date.Date == date.Date);
        if (excludingId.HasValue)
            spec.AddCriteria(w => w.Id != excludingId.Value);

        var existing = await workShiftRepository.FindAsync(spec, ct);

        if (existing.Any(w => w.StaffId == staffId && w.StartTime < endTime && startTime < w.EndTime))
            return (true, "staff member");

        if (equipmentId is { } equipment
            && existing.Any(w => w.EquipmentId == equipment && w.StartTime < endTime && startTime < w.EndTime))
            return (true, "equipment");

        return (false, string.Empty);
    }
}