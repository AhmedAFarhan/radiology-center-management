using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class ReferralFee : SoftDeletableAggregateRoot<Guid>
{
    public Guid ReferralDoctorId { get; private set; }
    public Guid ExaminationTypeId { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsActive { get; private set; }

    private ReferralFee()
    {
    }

    public static ReferralFee Create(
        Guid referralDoctorId,
        Guid examinationTypeId,
        decimal amount)
    {
        Guard.AgainstEmpty(referralDoctorId, nameof(referralDoctorId));
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.Against(amount, a => a < 0, "Referral fee cannot be negative.");

        var fee = new ReferralFee
        {
            Id = Guid.NewGuid(),
            ReferralDoctorId = referralDoctorId,
            ExaminationTypeId = examinationTypeId,
            Amount = amount,
            IsActive = true
        };

        return fee;
    }

    public void Update(decimal amount)
    {
        Guard.Against(amount, a => a < 0, "Referral fee cannot be negative.");

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
