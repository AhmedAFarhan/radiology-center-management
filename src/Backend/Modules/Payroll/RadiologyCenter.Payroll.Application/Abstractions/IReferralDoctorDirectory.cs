namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IReferralDoctorDirectory
{
    Task<bool> ExistsAsync(Guid referralDoctorId, CancellationToken ct = default);
}