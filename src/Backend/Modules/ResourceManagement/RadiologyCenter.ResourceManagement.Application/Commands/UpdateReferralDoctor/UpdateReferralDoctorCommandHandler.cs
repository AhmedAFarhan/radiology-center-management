using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public static class UpdateReferralDoctorCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateReferralDoctorCommand command,
        IReferralDoctorRepository referralDoctorRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var referralDoctor = await referralDoctorRepository.GetByIdAsync(command.ReferralDoctorId, ct);
        if (referralDoctor is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ReferralDoctorNotFound, "ReferralDoctor", command.ReferralDoctorId));

        var phone = command.Phone.Trim();

        var existing = await referralDoctorRepository.FindSingleAsync(
            new DynamicSpecification<ReferralDoctor>(rd => rd.Phone == phone && rd.Id != command.ReferralDoctorId), ct);
        if (existing is not null)
            return Result.Failure(
                Error.Conflict(ErrorCodes.ReferralDoctorPhoneExists, "A referral doctor with this phone number already exists."));

        referralDoctor.Update(
            command.FullName,
            phone,
            command.Email,
            command.Specialization,
            command.Hospital);

        referralDoctorRepository.Update(referralDoctor);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
