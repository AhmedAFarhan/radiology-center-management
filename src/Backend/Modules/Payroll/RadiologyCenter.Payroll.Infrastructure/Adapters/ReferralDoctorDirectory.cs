using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.Payroll.Infrastructure.Adapters;

public class ReferralDoctorDirectory : IReferralDoctorDirectory
{
    private readonly IReferralDoctorRepository _referralDoctorRepository;

    public ReferralDoctorDirectory(IReferralDoctorRepository referralDoctorRepository)
        => _referralDoctorRepository = referralDoctorRepository;

    public async Task<bool> ExistsAsync(Guid referralDoctorId, CancellationToken ct = default) =>
        await _referralDoctorRepository.GetByIdAsync(referralDoctorId, ct) is not null;
}
