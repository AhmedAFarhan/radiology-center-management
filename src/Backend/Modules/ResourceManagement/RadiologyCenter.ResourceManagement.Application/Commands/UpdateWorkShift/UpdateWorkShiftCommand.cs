namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;

public record UpdateWorkShiftCommand(
    Guid WorkShiftId,
    Guid StaffId,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    Guid? EquipmentId = null,
    string? Notes = null) : ICommand;
