using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateLeave;

public record CreateLeaveCommand(
    Guid StaffId,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason = null) : ICommand, ILeaveFields;
