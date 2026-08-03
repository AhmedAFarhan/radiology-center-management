namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public interface IWorkShiftFields
{
    Guid StaffId { get; }
    DateTime Date { get; }
    TimeSpan StartTime { get; }
    TimeSpan EndTime { get; }
    Guid? EquipmentId { get; }
    string? Notes { get; }
}
