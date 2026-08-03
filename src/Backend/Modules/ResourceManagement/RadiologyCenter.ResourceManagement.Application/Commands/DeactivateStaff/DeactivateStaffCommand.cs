namespace RadiologyCenter.ResourceManagement.Application.Commands.DeactivateStaff;

public record DeactivateStaffCommand(Guid StaffId) : ICommand;
