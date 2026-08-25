namespace RadiologyCenter.Payroll.Application.Abstractions;

public record ReferralFeeStatementDraft(
    Guid ReferralDoctorId,
    decimal TotalFee,
    int ExamCount);

public interface IReferralFeeStatementCalculator
{
    Task<IReadOnlyList<ReferralFeeStatementDraft>> CalculateAllAsync(
        DateTime from,
        DateTime to,
        CancellationToken ct = default);
}
