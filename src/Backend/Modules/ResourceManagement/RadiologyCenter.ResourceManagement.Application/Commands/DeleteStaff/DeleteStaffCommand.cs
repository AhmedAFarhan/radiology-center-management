namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteStaff;

public record DeleteStaffCommand(Guid StaffId) : ICommand;
