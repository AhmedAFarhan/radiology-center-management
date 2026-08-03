using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateReferralDoctor;

public static class CreateReferralDoctorCommandHandler
{
    public static async Task<Result<ReferralDoctorDto>> HandleAsync(
        CreateReferralDoctorCommand command,
        IReferralDoctorRepository referralDoctorRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var referralDoctor = ReferralDoctor.Create(
            command.Name,
            command.Phone,
            command.Email,
            command.Specialization,
            command.Hospital);

        await referralDoctorRepository.AddAsync(referralDoctor, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(referralDoctor.Adapt<ReferralDoctorDto>());
    }
}
