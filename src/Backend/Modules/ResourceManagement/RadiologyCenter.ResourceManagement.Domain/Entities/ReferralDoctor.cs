using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class ReferralDoctor : SoftDeletableAggregateRoot<Guid>
{
    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Specialization { get; private set; }
    public string? Hospital { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => string.Join(' ',
        new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));

    private ReferralDoctor()
    {
        FirstName = null!;
        LastName = null!;
        Phone = null!;
    }

    public static ReferralDoctor Create(
        string fullName,
        string phone,
        string? email = null,
        string? specialization = null,
        string? hospital = null)
    {
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phone, nameof(phone));

        var (firstName, middleName, lastName) = SplitFullName(fullName);

        var referralDoctor = new ReferralDoctor
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Phone = phone.Trim(),
            Email = email?.Trim(),
            Specialization = specialization?.Trim(),
            Hospital = hospital?.Trim(),
            IsActive = true
        };

        return referralDoctor;
    }

    public void Update(
        string fullName,
        string phone,
        string? email = null,
        string? specialization = null,
        string? hospital = null)
    {
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Guard.AgainstNullOrWhiteSpace(phone, nameof(phone));

        var (firstName, middleName, lastName) = SplitFullName(fullName);

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Phone = phone.Trim();
        Email = email?.Trim();
        Specialization = specialization?.Trim();
        Hospital = hospital?.Trim();
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
