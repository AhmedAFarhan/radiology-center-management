using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class ReferralFee : SoftDeletableAggregateRoot<Guid>
{
    public Guid ReferralDoctorId { get; private set; }
    public Guid ExaminationTypeId { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsPercentage { get; private set; }
    public bool IsActive { get; private set; }

    private ReferralFee()
    {
    }

    public static ReferralFee Create(
        Guid referralDoctorId,
        Guid examinationTypeId,
        decimal amount,
        bool isPercentage = false)
    {
        Guard.AgainstEmpty(referralDoctorId, nameof(referralDoctorId));
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.Against(amount, a => a < 0, "Referral fee cannot be negative.");
        if (isPercentage)
            Guard.Against(amount, a => a > 100, "Percentage fee cannot exceed 100.");

        var fee = new ReferralFee
        {
            Id = Guid.NewGuid(),
            ReferralDoctorId = referralDoctorId,
            ExaminationTypeId = examinationTypeId,
            Amount = amount,
            IsPercentage = isPercentage,
            IsActive = true
        };

        return fee;
    }

    public void Update(decimal amount, bool isPercentage = false)
    {
        Guard.Against(amount, a => a < 0, "Referral fee cannot be negative.");
        if (isPercentage)
            Guard.Against(amount, a => a > 100, "Percentage fee cannot exceed 100.");

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
