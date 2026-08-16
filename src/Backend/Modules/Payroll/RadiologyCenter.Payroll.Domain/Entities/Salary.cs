using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Domain.Errors;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class Salary : SoftDeletableAggregateRoot<Guid>
{
    public Guid StaffId { get; private set; }
    public decimal BaseSalary { get; private set; }
    public SalaryType SalaryType { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public bool IsActive { get; private set; }

    private Salary()
    {
        SalaryType = null!;
    }

    public static Salary Create(
        Guid staffId,
        decimal baseSalary,
        SalaryType salaryType,
        DateTime effectiveDate)
    {
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.Against(baseSalary, s => s < 0, DomainErrors.BaseSalaryNegative, "Base salary cannot be negative.");
        Guard.AgainstNull(salaryType, nameof(salaryType));
        Guard.AgainstDefault(effectiveDate, nameof(effectiveDate));

        var salary = new Salary
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            BaseSalary = baseSalary,
            SalaryType = salaryType,
            EffectiveDate = effectiveDate,
            IsActive = effectiveDate <= DateTime.UtcNow
        };

        return salary;
    }

    public void Update(decimal baseSalary, SalaryType salaryType, DateTime effectiveDate)
    {
        Guard.Against(baseSalary, s => s < 0, DomainErrors.BaseSalaryNegative, "Base salary cannot be negative.");
        Guard.AgainstNull(salaryType, nameof(salaryType));
        Guard.AgainstDefault(effectiveDate, nameof(effectiveDate));

        BaseSalary = baseSalary;
        SalaryType = salaryType;
        EffectiveDate = effectiveDate;
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
