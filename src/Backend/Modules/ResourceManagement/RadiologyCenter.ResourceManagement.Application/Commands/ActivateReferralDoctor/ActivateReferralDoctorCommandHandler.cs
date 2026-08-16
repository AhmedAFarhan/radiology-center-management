using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.ActivateReferralDoctor;

public static class ActivateReferralDoctorCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateReferralDoctorCommand command,
        IReferralDoctorRepository referralDoctorRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var referralDoctor = await referralDoctorRepository.GetByIdAsync(command.ReferralDoctorId, ct);
        if (referralDoctor is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ReferralDoctorNotFound, "ReferralDoctor", command.ReferralDoctorId));

        referralDoctor.Activate();
        referralDoctorRepository.Update(referralDoctor);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
