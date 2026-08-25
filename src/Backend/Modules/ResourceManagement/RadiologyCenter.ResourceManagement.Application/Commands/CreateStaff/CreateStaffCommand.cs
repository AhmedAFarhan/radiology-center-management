using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateStaff;

public record CreateStaffCommand(
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string Position,
    DateTime HireDate,
    string? Department = null,
    string? Specialization = null,
    string? LicenseNumber = null,
    string? SalaryCalculationRule = null) : ICommand, IStaffFields;
