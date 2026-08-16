using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Domain.Errors;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class AllowanceAssignment : SoftDeletableAggregateRoot<Guid>
{
    public Guid StaffId { get; private set; }
    public Guid? SalaryComponentId { get; private set; }
    public string Name { get; private set; }
    public decimal Amount { get; private set; }
    public Frequency? Frequency { get; private set; }
    public bool IsPerWorkDay { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }

    private AllowanceAssignment()
    {
        Name = null!;
    }

    public static AllowanceAssignment Create(
        Guid staffId,
        string name,
        decimal amount,
        DateTime effectiveDate,
        Guid? salaryComponentId = null,
        Frequency? frequency = null,
        DateTime? endDate = null,
        bool isPerWorkDay = false)
    {
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.Against(amount, a => a < 0, DomainErrors.AllowanceAmountNegative, "Allowance amount cannot be negative.");
        Guard.AgainstDefault(effectiveDate, nameof(effectiveDate));
        Guard.Against(endDate, d => d.HasValue && d < effectiveDate, DomainErrors.EndDateBeforeEffectiveDate, "End date cannot be before effective date.");

        var assignment = new AllowanceAssignment
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            SalaryComponentId = salaryComponentId,
            Name = name.Trim(),
            Amount = amount,
            Frequency = frequency,
            IsPerWorkDay = isPerWorkDay,
            EffectiveDate = effectiveDate,
            EndDate = endDate,
            IsActive = true
        };

        return assignment;
    }

    public void Update(
        string name,
        decimal amount,
        DateTime effectiveDate,
        Frequency? frequency = null,
        DateTime? endDate = null,
        bool isPerWorkDay = false)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.Against(amount, a => a < 0, DomainErrors.AllowanceAmountNegative, "Allowance amount cannot be negative.");
        Guard.AgainstDefault(effectiveDate, nameof(effectiveDate));
        Guard.Against(endDate, d => d.HasValue && d < effectiveDate, DomainErrors.EndDateBeforeEffectiveDate, "End date cannot be before effective date.");

        Name = name.Trim();
        Amount = amount;
        EffectiveDate = effectiveDate;
        EndDate = endDate;
        Frequency = frequency;
        IsPerWorkDay = isPerWorkDay;
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
