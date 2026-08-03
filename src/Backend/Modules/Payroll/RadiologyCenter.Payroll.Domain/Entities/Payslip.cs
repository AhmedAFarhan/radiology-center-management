using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class Payslip : Entity<Guid>
{
    private readonly List<PayslipComponent> _components = [];

    public Guid PayRunId { get; private set; }
    public Guid StaffId { get; private set; }
    public decimal GrossSalary { get; private set; }
    public int UnpaidLeaveDays { get; private set; }
    public decimal UnpaidLeaveDeduction { get; private set; }
    public string? Notes { get; private set; }

    public decimal TotalEarnings => _components.Where(c => !c.IsDeduction).Sum(c => c.Amount);
    public decimal TotalDeductions => _components.Where(c => c.IsDeduction).Sum(c => c.Amount) + UnpaidLeaveDeduction;
    public decimal NetSalary => GrossSalary + TotalEarnings - TotalDeductions;
    public IReadOnlyCollection<PayslipComponent> Components => _components.AsReadOnly();

    private Payslip() { }

    public static Payslip Create(
        Guid payRunId,
        Guid staffId,
        decimal grossSalary,
        int unpaidLeaveDays = 0,
        decimal unpaidLeaveDeduction = 0,
        string? notes = null)
    {
        Guard.AgainstEmpty(payRunId, nameof(payRunId));
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.Against(grossSalary, g => g < 0, "Gross salary cannot be negative.");
        Guard.Against(unpaidLeaveDays, d => d < 0, "Unpaid leave days cannot be negative.");
        Guard.Against(unpaidLeaveDeduction, d => d < 0, "Unpaid leave deduction cannot be negative.");

        return new Payslip
        {
            Id = Guid.NewGuid(),
            PayRunId = payRunId,
            StaffId = staffId,
            GrossSalary = grossSalary,
            UnpaidLeaveDays = unpaidLeaveDays,
            UnpaidLeaveDeduction = unpaidLeaveDeduction,
            Notes = notes?.Trim()
        };
    }

    public PayslipComponent AddComponent(string name, decimal amount, bool isDeduction = false)
    {
        var component = PayslipComponent.Create(Id, name, amount, isDeduction);
        _components.Add(component);
        return component;
    }

    public void RemoveComponent(Guid componentId)
    {
        var component = _components.FirstOrDefault(c => c.Id == componentId)
            ?? throw new DomainException($"Payslip component '{componentId}' not found on payslip '{Id}'.");
        _components.Remove(component);
    }
}
