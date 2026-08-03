using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;

public record UpdateStaffCommand(
    Guid StaffId,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string Position,
    DateTime HireDate,
    string? Department = null,
    string? Specialization = null,
    string? LicenseNumber = null) : ICommand, IStaffFields;
