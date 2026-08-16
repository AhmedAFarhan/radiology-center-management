using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.ResourceManagement.Domain.Errors;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class WorkShift : SoftDeletableAggregateRoot<Guid>
{
    public Guid StaffId { get; private set; }
    public Guid? EquipmentId { get; private set; }
    public DateTime Date { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public string? Notes { get; private set; }

    private WorkShift() { }

    public static WorkShift Create(
        Guid staffId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? equipmentId = null,
        string? notes = null)
    {
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.AgainstDefault(date, nameof(date));
        Guard.Against(startTime, t => t >= endTime, DomainErrors.StartTimeBeforeEndTime, "Start time must be before end time.");

        var workShift = new WorkShift
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            EquipmentId = equipmentId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            Notes = notes?.Trim()
        };

        return workShift;
    }

    public void Update(
        Guid staffId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? equipmentId = null,
        string? notes = null)
    {
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.AgainstDefault(date, nameof(date));
        Guard.Against(startTime, t => t >= endTime, DomainErrors.StartTimeBeforeEndTime, "Start time must be before end time.");

        StaffId = staffId;
        EquipmentId = equipmentId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Notes = notes?.Trim();
    }
}
