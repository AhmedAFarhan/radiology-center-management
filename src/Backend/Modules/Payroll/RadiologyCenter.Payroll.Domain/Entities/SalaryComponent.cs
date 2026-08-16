using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Domain.Errors;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class SalaryComponent : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public ComponentKind Kind { get; private set; }
    public Frequency? Frequency { get; private set; }
    public bool IsPercentage { get; private set; }
    public bool IsPerWorkDay { get; private set; }
    public decimal DefaultValue { get; private set; }
    public bool IsActive { get; private set; }

    private SalaryComponent()
    {
        Name = null!;
        Kind = null!;
    }

    public static SalaryComponent Create(
        string name,
        ComponentKind kind,
        bool isPercentage = false,
        decimal defaultValue = 0,
        Frequency? frequency = null,
        bool isPerWorkDay = false)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(kind, nameof(kind));
        Guard.Against(defaultValue, d => d < 0, DomainErrors.DefaultValueNegative, "Default value cannot be negative.");

        var component = new SalaryComponent
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Kind = kind,
            IsPercentage = isPercentage,
            IsPerWorkDay = isPerWorkDay,
            DefaultValue = defaultValue,
            Frequency = frequency,
            IsActive = true
        };

        return component;
    }

    public void Update(string name, ComponentKind kind, bool isPercentage, decimal defaultValue, Frequency? frequency = null, bool isPerWorkDay = false)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(kind, nameof(kind));
        Guard.Against(defaultValue, d => d < 0, DomainErrors.DefaultValueNegative, "Default value cannot be negative.");

        Name = name.Trim();
        Kind = kind;
        IsPercentage = isPercentage;
        IsPerWorkDay = isPerWorkDay;
        DefaultValue = defaultValue;
        Frequency = frequency;
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
