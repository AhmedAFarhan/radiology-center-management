using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class Staff : SoftDeletableAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public StaffPosition Position { get; private set; }
    public string? Department { get; private set; }
    public string? Specialization { get; private set; }
    public string? LicenseNumber { get; private set; }
    public DateTime HireDate { get; private set; }
    public SalaryCalculationRule SalaryCalculationRule { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => string.Join(' ',
        new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));

    private Staff()
    {
        FirstName = null!;
        LastName = null!;
        PhoneNumber = null!;
        Position = null!;
        SalaryCalculationRule = SalaryCalculationRule.FixedPlusFees;
    }

    public static Staff Create(
        Guid userId,
        string fullName,
        string phoneNumber,
        StaffPosition position,
        DateTime hireDate,
        string? department = null,
        string? specialization = null,
        string? licenseNumber = null,
        SalaryCalculationRule? salaryCalculationRule = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(position, nameof(position));
        Guard.AgainstDefault(hireDate, nameof(hireDate));

        var (firstName, middleName, lastName) = PersonName.Split(fullName);

        var staff = new Staff
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            PhoneNumber = phoneNumber.Trim(),
            Position = position,
            Department = department?.Trim(),
            Specialization = specialization?.Trim(),
            LicenseNumber = licenseNumber?.Trim(),
            HireDate = hireDate,
            SalaryCalculationRule = salaryCalculationRule ?? SalaryCalculationRule.FixedPlusFees,
            IsActive = true
        };

        return staff;
    }

    public void Update(
        Guid userId,
        string fullName,
        string phoneNumber,
        StaffPosition position,
        DateTime hireDate,
        string? department = null,
        string? specialization = null,
        string? licenseNumber = null,
        SalaryCalculationRule? salaryCalculationRule = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(position, nameof(position));
        Guard.AgainstDefault(hireDate, nameof(hireDate));

        var (firstName, middleName, lastName) = PersonName.Split(fullName);

        UserId = userId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber.Trim();
        Position = position;
        Department = department?.Trim();
        Specialization = specialization?.Trim();
        LicenseNumber = licenseNumber?.Trim();
        HireDate = hireDate;
        SalaryCalculationRule = salaryCalculationRule ?? SalaryCalculationRule.FixedPlusFees;
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
}
