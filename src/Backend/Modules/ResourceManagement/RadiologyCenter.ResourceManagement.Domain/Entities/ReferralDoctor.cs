using RadiologyCenter.BuildingBlocks.Domain.Common;
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

        var (firstName, middleName, lastName) = PersonName.Split(fullName);

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

        var (firstName, middleName, lastName) = PersonName.Split(fullName);

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
}
