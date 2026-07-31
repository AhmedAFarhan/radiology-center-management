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
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
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

    public string FullName => $"{FirstName} {LastName}".Trim();

    public int Age => CalculateAge();

    private Patient()
    {
        PatientCode = null!;
        FirstName = null!;
        LastName = null!;
        Gender = null!;
    }

    public static Patient Create(
        string patientCode,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        Gender gender,
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
        Guard.AgainstNullOrWhiteSpace(firstName, nameof(firstName));
        Guard.AgainstNullOrWhiteSpace(lastName, nameof(lastName));
        Guard.AgainstDefault(dateOfBirth, nameof(dateOfBirth));
        Guard.AgainstNull(gender, nameof(gender));
        Guard.Against(dateOfBirth, d => d.Date > DateTime.UtcNow.Date, "Date of birth cannot be in the future.");

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            PatientCode = patientCode,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
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

        patient.RaiseDomainEvent(new PatientRegisteredEvent(patient.Id, patient.PatientCode, patient.FullName));
        return patient;
    }

    public void Update(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        Gender gender,
        string? phoneNumber = null,
        string? email = null,
        string? address = null,
        string? nationalId = null,
        BloodType? bloodType = null,
        string? allergies = null,
        string? medicalHistory = null,
        string? referringPhysician = null)
    {
        Guard.AgainstNullOrWhiteSpace(firstName, nameof(firstName));
        Guard.AgainstNullOrWhiteSpace(lastName, nameof(lastName));
        Guard.AgainstDefault(dateOfBirth, nameof(dateOfBirth));
        Guard.AgainstNull(gender, nameof(gender));
        Guard.Against(dateOfBirth, d => d.Date > DateTime.UtcNow.Date, "Date of birth cannot be in the future.");

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        NationalId = nationalId;
        BloodType = bloodType;
        Allergies = allergies;
        MedicalHistory = medicalHistory;
        ReferringPhysician = referringPhysician;

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

    private int CalculateAge()
    {
        var today = DateTime.UtcNow.Date;
        var birthDate = DateOfBirth.Date;

        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
            age--;

        return Math.Max(age, 0);
    }
}
