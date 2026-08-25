namespace RadiologyCenter.Payroll.Application.Abstractions;

public record ReferralFeeExamBreakdownItem(
    string ExaminationTypeName,
    int Count,
    decimal TotalFee);

public record ReferralFeeExamBreakdown(
    Guid ReferralDoctorId,
    decimal TotalFee,
    int ExamCount,
    IReadOnlyList<ReferralFeeExamBreakdownItem> Items);

public interface IReferralFeeStatementResolver
{
    Task<IReadOnlyList<ReferralFeeExamBreakdown>> GetReferralFeeBreakdownsAsync(
        DateTime from,
        DateTime to,
        CancellationToken ct = default);

    Task<ReferralFeeExamBreakdown?> GetReferralFeeBreakdownAsync(
        Guid referralDoctorId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default);
}
