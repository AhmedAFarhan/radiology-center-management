namespace RadiologyCenter.Desktop.Features.Patients.Models;

public sealed record PatientDto(
    string Id,
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
    string? BloodTypeKey = null);

public sealed class PatientInput
{
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
}