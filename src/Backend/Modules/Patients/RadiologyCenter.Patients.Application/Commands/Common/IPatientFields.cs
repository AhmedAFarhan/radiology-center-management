namespace RadiologyCenter.Patients.Application.Commands.Common;

public interface IPatientFields
{
    string FullName { get; }
    string Gender { get; }
    DateTime? DateOfBirth { get; }
    int? Age { get; }
    string PhoneNumber { get; }
    string? Email { get; }
    string? Address { get; }
    string? NationalId { get; }
    string? BloodType { get; }
    string? Allergies { get; }
    string? MedicalHistory { get; }
    string? ReferringPhysician { get; }
}
