using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
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
        var phone = command.Phone.Trim();

        var existing = await referralDoctorRepository.FindSingleAsync(
            new DynamicSpecification<ReferralDoctor>(rd => rd.Phone == phone), ct);
        if (existing is not null)
            return Result.Failure<ReferralDoctorDto>(
                Error.Conflict("A referral doctor with this phone number already exists."));

        var referralDoctor = ReferralDoctor.Create(
            command.FullName,
            phone,
            command.Email,
            command.Specialization,
            command.Hospital);

        await referralDoctorRepository.AddAsync(referralDoctor, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(referralDoctor.Adapt<ReferralDoctorDto>());
    }
}
