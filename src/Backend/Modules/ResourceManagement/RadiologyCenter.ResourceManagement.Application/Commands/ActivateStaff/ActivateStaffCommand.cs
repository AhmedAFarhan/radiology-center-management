namespace RadiologyCenter.ResourceManagement.Application.Commands.ActivateStaff;

public record ActivateStaffCommand(Guid StaffId) : ICommand;
