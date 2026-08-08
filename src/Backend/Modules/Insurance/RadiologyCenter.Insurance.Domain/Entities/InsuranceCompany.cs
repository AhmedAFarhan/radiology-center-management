using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class InsuranceCompany : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? TaxId { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    private InsuranceCompany()
    {
        Name = string.Empty;
    }

    public static InsuranceCompany Create(
        string name,
        string? taxId = null,
        string? address = null,
        string? phone = null,
        string? email = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        return new InsuranceCompany
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            TaxId = taxId?.Trim(),
            Address = address?.Trim(),
            Phone = phone?.Trim(),
            Email = email?.Trim()
        };
    }

    public void Update(
        string name,
        string? taxId = null,
        string? address = null,
        string? phone = null,
        string? email = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        Name = name.Trim();
        TaxId = taxId?.Trim();
        Address = address?.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
    }
}