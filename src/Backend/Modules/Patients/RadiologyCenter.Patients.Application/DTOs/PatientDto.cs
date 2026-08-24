namespace RadiologyCenter.Patients.Application.DTOs;

public record PatientDto(
    Guid Id,
    string PatientCode,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    DateTime? DateOfBirth,
    int? Age,
    string Gender,
    string PhoneNumber,
    string? Email,
    string? Address,
    string? NationalId,
    string? BloodType,
    string? Allergies,
    string? MedicalHistory,
    bool IsActive,
    DateTime CreatedAt,
    string GenderKey = "",
    string? BloodTypeKey = null
);
