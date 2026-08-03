using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctors;

public static class GetReferralDoctorsQueryHandler
{
    public static async Task<Result<PagedResult<ReferralDoctorDto>>> HandleAsync(
        GetReferralDoctorsQuery query,
        IReferralDoctorRepository referralDoctorRepository,
        CancellationToken ct)
    {
        var paged = await referralDoctorRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(r => r.Adapt<ReferralDoctorDto>()).ToList();

        return Result.Success(new PagedResult<ReferralDoctorDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
