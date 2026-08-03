namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteWorkShift;

public record DeleteWorkShiftCommand(Guid WorkShiftId) : ICommand;
