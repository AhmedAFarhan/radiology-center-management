using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class ReferralDoctor : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Specialization { get; private set; }
    public string? Hospital { get; private set; }
    public bool IsActive { get; private set; }

    private ReferralDoctor()
    {
        Name = null!;
        Phone = null!;
    }

    public static ReferralDoctor Create(
        string name,
        string phone,
        string? email = null,
        string? specialization = null,
        string? hospital = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNullOrWhiteSpace(phone, nameof(phone));

        var referralDoctor = new ReferralDoctor
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Phone = phone.Trim(),
            Email = email?.Trim(),
            Specialization = specialization?.Trim(),
            Hospital = hospital?.Trim(),
            IsActive = true
        };

        return referralDoctor;
    }

    public void Update(
        string name,
        string phone,
        string? email = null,
        string? specialization = null,
        string? hospital = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNullOrWhiteSpace(phone, nameof(phone));

        Name = name.Trim();
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
