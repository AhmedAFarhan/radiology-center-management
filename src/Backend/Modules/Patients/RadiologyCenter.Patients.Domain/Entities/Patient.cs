using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Patients.Domain.Enumerations;
using RadiologyCenter.Patients.Domain.Errors;
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
    public string PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? NationalId { get; private set; }
    public BloodType? BloodType { get; private set; }
    public string? Allergies { get; private set; }
    public string? MedicalHistory { get; private set; }
    public string? ReferringPhysician { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => string.Join(' ',
        new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));

    public int? Age { get; private set; }

    private Patient()
    {
        PatientCode = null!;
        FirstName = null!;
        LastName = null!;
        Gender = null!;
        PhoneNumber = null!;
    }

    public static Patient Create(
        string patientCode,
        string fullName,
        Gender gender,
        DateTime? dateOfBirth = null,
        int? age = null,
        string phoneNumber = null!,
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
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(gender, nameof(gender));
        ValidateBirthDetails(dateOfBirth, age);

        var (firstName, middleName, lastName) = PersonName.Split(fullName);

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
        string phoneNumber = null!,
        string? email = null,
        string? address = null,
        string? nationalId = null,
        BloodType? bloodType = null,
        string? allergies = null,
        string? medicalHistory = null,
        string? referringPhysician = null)
    {
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(gender, nameof(gender));
        ValidateBirthDetails(dateOfBirth, age);

        var (firstName, middleName, lastName) = PersonName.Split(fullName);

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
        Age = dateOfBirth.HasValue ? CalculateAge(dateOfBirth.Value) : age;
    }

    private static void ValidateBirthDetails(DateTime? dateOfBirth, int? age)
    {
        if (dateOfBirth is null && age is null)
            throw new DomainException(DomainErrors.DobOrAgeRequired, "Either date of birth or age must be provided.");

        if (dateOfBirth is not null)
            Guard.Against(dateOfBirth.Value, d => d.Date > DateTime.UtcNow.Date, DomainErrors.DateOfBirthFuture, "Date of birth cannot be in the future.");

        if (age is not null)
            Guard.Against(age.Value, a => a is < 0 or > 150, DomainErrors.AgeOutOfRange, "Age must be between 0 and 150.");
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
