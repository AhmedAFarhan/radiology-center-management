using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class PayslipComponent : Entity<Guid>
{
    public Guid PayslipId { get; private set; }
    public string Name { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsDeduction { get; private set; }

    private PayslipComponent()
    {
        Name = null!;
    }

    public static PayslipComponent Create(
        Guid payslipId,
        string name,
        decimal amount,
        bool isDeduction = false)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        return new PayslipComponent
        {
            Id = Guid.NewGuid(),
            PayslipId = payslipId,
            Name = name.Trim(),
            Amount = amount,
            IsDeduction = isDeduction
        };
    }
}
