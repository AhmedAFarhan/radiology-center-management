using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Patients.Domain.Enumerations;
using RadiologyCenter.Patients.Domain.Events;

namespace RadiologyCenter.Patients.Domain.Entities;

public sealed class Patient : SoftDeletableAggregateRoot<Guid>
{
    public string PatientCode { get; private set; }
    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? NationalId { get; private set; }
    public BloodType? BloodType { get; private set; }
    public string? Allergies { get; private set; }
    public string? MedicalHistory { get; private set; }
    public string? ReferringPhysician { get; private set; }
    public bool IsActive { get; private set; }

    private int? _age;

    public string FullName => string.Join(' ',
        new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));

    public int? Age => DateOfBirth.HasValue ? CalculateAge(DateOfBirth.Value) : _age;

    private Patient()
    {
        PatientCode = null!;
        FirstName = null!;
        LastName = null!;
        Gender = null!;
    }

    public static Patient Create(
        string patientCode,
        string fullName,
        Gender gender,
        DateTime? dateOfBirth = null,
        int? age = null,
        string? phoneNumber = null,
        string? email = null,
        string? address = null,
        string? nationalId = null,
        BloodType? bloodType = null,
        string? allergies = null,
        string? medicalHistory = null,
        string? referringPhysician = null)
    {
        Guard.AgainstNullOrWhiteSpace(patientCode, nameof(patientCode));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNull(gender, nameof(gender));
        ValidateBirthDetails(dateOfBirth, age);

        var (firstName, middleName, lastName) = SplitFullName(fullName);

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            PatientCode = patientCode,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Gender = gender,
            PhoneNumber = phoneNumber,
            Email = email,
            Address = address,
            NationalId = nationalId,
            BloodType = bloodType,
            Allergies = allergies,
            MedicalHistory = medicalHistory,
            ReferringPhysician = referringPhysician,
            IsActive = true
        };

        patient.SetBirthDetails(dateOfBirth, age);
        patient.RaiseDomainEvent(new PatientRegisteredEvent(patient.Id, patient.PatientCode, patient.FullName));
        return patient;
    }

    public void Update(
        string fullName,
        Gender gender,
        DateTime? dateOfBirth = null,
        int? age = null,
        string? phoneNumber = null,
        string? email = null,
        string? address = null,
        string? nationalId = null,
        BloodType? bloodType = null,
        string? allergies = null,
        string? medicalHistory = null,
        string? referringPhysician = null)
    {
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNull(gender, nameof(gender));
        ValidateBirthDetails(dateOfBirth, age);

        var (firstName, middleName, lastName) = SplitFullName(fullName);

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        NationalId = nationalId;
        BloodType = bloodType;
        Allergies = allergies;
        MedicalHistory = medicalHistory;
        ReferringPhysician = referringPhysician;
        SetBirthDetails(dateOfBirth, age);

        RaiseDomainEvent(new PatientUpdatedEvent(Id, PatientCode));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }

    private void SetBirthDetails(DateTime? dateOfBirth, int? age)
    {
        DateOfBirth = dateOfBirth;
        _age = dateOfBirth.HasValue ? CalculateAge(dateOfBirth.Value) : age;
    }

    private static (string FirstName, string? MiddleName, string LastName) SplitFullName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
            throw new DomainException("Full name must contain at least a first name and a last name.");

        var firstName = parts[0];
        var lastName = parts[^1];
        var middleName = parts.Length > 2 ? string.Join(' ', parts[1..^1]) : null;

        return (firstName, middleName, lastName);
    }

    private static void ValidateBirthDetails(DateTime? dateOfBirth, int? age)
    {
        if (dateOfBirth is null && age is null)
            throw new DomainException("Either date of birth or age must be provided.");

        if (dateOfBirth is not null)
            Guard.Against(dateOfBirth.Value, d => d.Date > DateTime.UtcNow.Date, "Date of birth cannot be in the future.");

        if (age is not null)
            Guard.Against(age.Value, a => a is < 0 or > 150, "Age must be between 0 and 150.");
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        var birthDate = dateOfBirth.Date;

        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
            age--;

        return Math.Max(age, 0);
    }
}
