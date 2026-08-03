using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateWorkShift;

public record CreateWorkShiftCommand(
    Guid StaffId,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    Guid? EquipmentId = null,
    string? Notes = null) : ICommand, IWorkShiftFields;
