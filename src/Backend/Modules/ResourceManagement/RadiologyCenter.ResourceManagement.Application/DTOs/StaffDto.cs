namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record StaffDto(
    Guid Id,
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    string Position,
    string? Department,
    string? Specialization,
    string? LicenseNumber,
    DateTime HireDate,
    bool IsActive,
    DateTime CreatedAt,
    string PositionKey = "");
