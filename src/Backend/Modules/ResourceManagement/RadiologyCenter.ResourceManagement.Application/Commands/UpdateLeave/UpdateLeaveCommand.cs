namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;

public record UpdateLeaveCommand(
    Guid LeaveId,
    Guid StaffId,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason = null) : ICommand;
