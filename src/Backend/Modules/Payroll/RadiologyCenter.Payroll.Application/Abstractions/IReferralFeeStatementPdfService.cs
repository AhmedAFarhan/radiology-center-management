namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IReferralFeeStatementPdfService
{
    Task<byte[]> GenerateStatementPdfAsync(
        Guid payRunId,
        Guid referralDoctorId,
        CancellationToken ct = default);
}
