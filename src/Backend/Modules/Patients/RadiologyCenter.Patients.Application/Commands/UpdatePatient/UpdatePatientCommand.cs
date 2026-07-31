namespace RadiologyCenter.Patients.Application.Commands.UpdatePatient;

public record UpdatePatientCommand(
    Guid PatientId,
    string FullName,
    string Gender,
    DateTime? DateOfBirth,
    int? Age,
    string PhoneNumber,
    string? Email = null,
    string? Address = null,
    string? NationalId = null,
    string? BloodType = null,
    string? Allergies = null,
    string? MedicalHistory = null,
    string? ReferringPhysician = null) : ICommand;
