using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class ExaminationFee : SoftDeletableAggregateRoot<Guid>
{
    public Guid ExaminationTypeId { get; private set; }
    public ExamFeeRole Role { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsActive { get; private set; }

    private ExaminationFee()
    {
        Role = null!;
    }

    public static ExaminationFee Create(
        Guid examinationTypeId,
        ExamFeeRole role,
        decimal amount)
    {
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.AgainstNull(role, nameof(role));
        Guard.Against(amount, a => a < 0, "Examination fee cannot be negative.");

        var fee = new ExaminationFee
        {
            Id = Guid.NewGuid(),
            ExaminationTypeId = examinationTypeId,
            Role = role,
            Amount = amount,
            IsActive = true
        };

        return fee;
    }

    public void Update(ExamFeeRole role, decimal amount)
    {
        Guard.AgainstNull(role, nameof(role));
        Guard.Against(amount, a => a < 0, "Examination fee cannot be negative.");

        Role = role;
        Amount = amount;
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
