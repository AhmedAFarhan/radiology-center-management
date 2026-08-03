using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class Staff : SoftDeletableAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string EmployeeNumber { get; private set; }
    public string PhoneNumber { get; private set; }
    public StaffPosition Position { get; private set; }
    public string? Department { get; private set; }
    public string? Specialization { get; private set; }
    public string? LicenseNumber { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; private set; }

    private Staff()
    {
        EmployeeNumber = null!;
        PhoneNumber = null!;
        Position = null!;
    }

    public static Staff Create(
        Guid userId,
        string employeeNumber,
        string phoneNumber,
        StaffPosition position,
        DateTime hireDate,
        string? department = null,
        string? specialization = null,
        string? licenseNumber = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(employeeNumber, nameof(employeeNumber));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(position, nameof(position));
        Guard.AgainstDefault(hireDate, nameof(hireDate));

        var staff = new Staff
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EmployeeNumber = employeeNumber.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Position = position,
            Department = department?.Trim(),
            Specialization = specialization?.Trim(),
            LicenseNumber = licenseNumber?.Trim(),
            HireDate = hireDate,
            IsActive = true
        };

        return staff;
    }

    public void Update(
        Guid userId,
        string employeeNumber,
        string phoneNumber,
        StaffPosition position,
        DateTime hireDate,
        string? department = null,
        string? specialization = null,
        string? licenseNumber = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(employeeNumber, nameof(employeeNumber));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(position, nameof(position));
        Guard.AgainstDefault(hireDate, nameof(hireDate));

        UserId = userId;
        EmployeeNumber = employeeNumber.Trim();
        PhoneNumber = phoneNumber.Trim();
        Position = position;
        Department = department?.Trim();
        Specialization = specialization?.Trim();
        LicenseNumber = licenseNumber?.Trim();
        HireDate = hireDate;
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
