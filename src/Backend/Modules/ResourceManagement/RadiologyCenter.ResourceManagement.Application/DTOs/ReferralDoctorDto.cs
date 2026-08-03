namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record ReferralDoctorDto(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string? Specialization,
    string? Hospital,
    bool IsActive,
    DateTime CreatedAt);
