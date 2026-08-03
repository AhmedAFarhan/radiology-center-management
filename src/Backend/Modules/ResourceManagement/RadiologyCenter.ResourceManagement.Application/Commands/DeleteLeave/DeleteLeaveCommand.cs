namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteLeave;

public record DeleteLeaveCommand(Guid LeaveId) : ICommand;
