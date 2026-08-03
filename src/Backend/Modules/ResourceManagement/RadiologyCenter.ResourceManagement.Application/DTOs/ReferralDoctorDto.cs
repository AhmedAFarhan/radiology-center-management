namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record ReferralDoctorDto(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string Phone,
    string? Email,
    string? Specialization,
    string? Hospital,
    bool IsActive,
    DateTime CreatedAt);
