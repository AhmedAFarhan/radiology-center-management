using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.Inventory.Domain.Entities;

public sealed class Supplier : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? ContactPerson { get; private set; }
    public string Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? PaymentTerms { get; private set; }
    public bool IsActive { get; private set; }

    private Supplier()
    {
        Name = null!;
        Phone = null!;
    }

    public static Supplier Create(
        string name,
        string phone,
        string? contactPerson = null,
        string? email = null,
        string? address = null,
        string? taxNumber = null,
        string? paymentTerms = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNullOrWhiteSpace(phone, nameof(phone));

        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            ContactPerson = contactPerson?.Trim(),
            Phone = phone.Trim(),
            Email = email?.Trim(),
            Address = address?.Trim(),
            TaxNumber = taxNumber?.Trim(),
            PaymentTerms = paymentTerms?.Trim(),
            IsActive = true
        };

        return supplier;
    }

    public void Update(
        string name,
        string phone,
        string? contactPerson = null,
        string? email = null,
        string? address = null,
        string? taxNumber = null,
        string? paymentTerms = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNullOrWhiteSpace(phone, nameof(phone));

        Name = name.Trim();
        ContactPerson = contactPerson?.Trim();
        Phone = phone.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        TaxNumber = taxNumber?.Trim();
        PaymentTerms = paymentTerms?.Trim();
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
