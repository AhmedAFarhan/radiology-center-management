namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateStaff;

public record CreateStaffCommand(
    Guid UserId,
    string EmployeeNumber,
    string PhoneNumber,
    string Position,
    DateTime HireDate,
    string? Department = null,
    string? Specialization = null,
    string? LicenseNumber = null) : ICommand;
