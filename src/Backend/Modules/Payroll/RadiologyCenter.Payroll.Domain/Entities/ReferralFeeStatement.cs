using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Payroll.Domain.Errors;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class ReferralFeeStatement : Entity<Guid>
{
    public Guid PayRunId { get; private set; }
    public Guid ReferralDoctorId { get; private set; }
    public decimal TotalFee { get; private set; }
    public int ExamCount { get; private set; }

    private ReferralFeeStatement() { }

    public static ReferralFeeStatement Create(
        Guid payRunId,
        Guid referralDoctorId,
        decimal totalFee,
        int examCount)
    {
        Guard.AgainstEmpty(payRunId, nameof(payRunId));
        Guard.AgainstEmpty(referralDoctorId, nameof(referralDoctorId));
        Guard.Against(totalFee, f => f < 0, DomainErrors.ReferralFeeNegative, "Total fee cannot be negative.");
        Guard.Against(examCount, c => c < 0, DomainErrors.DefaultValueNegative, "Exam count cannot be negative.");

        return new ReferralFeeStatement
        {
            Id = Guid.NewGuid(),
            PayRunId = payRunId,
            ReferralDoctorId = referralDoctorId,
            TotalFee = totalFee,
            ExamCount = examCount
        };
    }
}
