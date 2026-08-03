using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteReferralDoctor;

public static class DeleteReferralDoctorCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteReferralDoctorCommand command,
        IReferralDoctorRepository referralDoctorRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var referralDoctor = await referralDoctorRepository.GetByIdAsync(command.ReferralDoctorId, ct);
        if (referralDoctor is null)
            return Result.Failure(Error.NotFound("ReferralDoctor", command.ReferralDoctorId));

        referralDoctorRepository.Remove(referralDoctor);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
