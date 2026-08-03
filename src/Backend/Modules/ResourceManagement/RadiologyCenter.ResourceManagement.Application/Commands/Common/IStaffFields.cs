namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public interface IStaffFields
{
    Guid UserId { get; }
    string FullName { get; }
    string PhoneNumber { get; }
    string Position { get; }
    DateTime HireDate { get; }
    string? Department { get; }
    string? Specialization { get; }
    string? LicenseNumber { get; }
}
