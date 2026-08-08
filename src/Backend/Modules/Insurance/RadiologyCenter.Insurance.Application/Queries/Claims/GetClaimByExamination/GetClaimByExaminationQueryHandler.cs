using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimByExamination;

public static class GetClaimByExaminationQueryHandler
{
    public static async Task<Result<ClaimDto>> HandleAsync(
        GetClaimByExaminationQuery query,
        IClaimRepository claimRepository,
        CancellationToken ct)
    {
        var claim = await claimRepository.GetByExaminationIdAsync(query.ExaminationId, ct);
        return claim is null
            ? Result.Failure<ClaimDto>(Error.NotFound("Claim", query.ExaminationId))
            : Result.Success(claim.ToDto());
    }
}