namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record StaffDto(
    Guid Id,
    Guid UserId,
    string EmployeeNumber,
    string PhoneNumber,
    string Position,
    string? Department,
    string? Specialization,
    string? LicenseNumber,
    DateTime HireDate,
    bool IsActive,
    DateTime CreatedAt);
