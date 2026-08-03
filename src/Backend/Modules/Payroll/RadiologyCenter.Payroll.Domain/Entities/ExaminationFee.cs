using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class ExaminationFee : SoftDeletableAggregateRoot<Guid>
{
    public Guid ExaminationTypeId { get; private set; }
    public ExamFeeRole Role { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsPercentage { get; private set; }
    public bool IsActive { get; private set; }

    private ExaminationFee()
    {
        Role = null!;
    }

    public static ExaminationFee Create(
        Guid examinationTypeId,
        ExamFeeRole role,
        decimal amount,
        bool isPercentage = false)
    {
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.AgainstNull(role, nameof(role));
        Guard.Against(amount, a => a < 0, "Examination fee cannot be negative.");
        if (isPercentage)
            Guard.Against(amount, a => a > 100, "Percentage fee cannot exceed 100.");

        var fee = new ExaminationFee
        {
            Id = Guid.NewGuid(),
            ExaminationTypeId = examinationTypeId,
            Role = role,
            Amount = amount,
            IsPercentage = isPercentage,
            IsActive = true
        };

        return fee;
    }

    public void Update(ExamFeeRole role, decimal amount, bool isPercentage = false)
    {
        Guard.AgainstNull(role, nameof(role));
        Guard.Against(amount, a => a < 0, "Examination fee cannot be negative.");
        if (isPercentage)
            Guard.Against(amount, a => a > 100, "Percentage fee cannot exceed 100.");

        Role = role;
        Amount = amount;
        IsPercentage = isPercentage;
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
