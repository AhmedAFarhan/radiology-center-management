using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
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
    public bool IsActive { get; private set; }

    public string FullName => string.Join(' ',
        new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));

    private Staff()
    {
        FirstName = null!;
        LastName = null!;
        PhoneNumber = null!;
        Position = null!;
    }

    public static Staff Create(
        Guid userId,
        string fullName,
        string phoneNumber,
        StaffPosition position,
        DateTime hireDate,
        string? department = null,
        string? specialization = null,
        string? licenseNumber = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(position, nameof(position));
        Guard.AgainstDefault(hireDate, nameof(hireDate));

        var (firstName, middleName, lastName) = SplitFullName(fullName);

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
        string? licenseNumber = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNull(position, nameof(position));
        Guard.AgainstDefault(hireDate, nameof(hireDate));

        var (firstName, middleName, lastName) = SplitFullName(fullName);

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
}
