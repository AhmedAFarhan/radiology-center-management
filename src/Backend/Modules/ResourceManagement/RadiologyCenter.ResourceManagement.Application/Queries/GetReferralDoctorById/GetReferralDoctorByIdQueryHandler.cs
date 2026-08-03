using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctorById;

public static class GetReferralDoctorByIdQueryHandler
{
    public static async Task<Result<ReferralDoctorDto>> HandleAsync(
        GetReferralDoctorByIdQuery query,
        IReferralDoctorRepository referralDoctorRepository,
        CancellationToken ct)
    {
        var referralDoctor = await referralDoctorRepository.GetByIdAsync(query.Id, ct);
        if (referralDoctor is null)
            return Result.Failure<ReferralDoctorDto>(Error.NotFound("ReferralDoctor", query.Id));

        return Result.Success(referralDoctor.Adapt<ReferralDoctorDto>());
    }
}
