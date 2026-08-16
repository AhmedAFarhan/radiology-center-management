using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimById;

public static class GetClaimByIdQueryHandler
{
    public static async Task<Result<ClaimDto>> HandleAsync(
        GetClaimByIdQuery query,
        IClaimRepository claimRepository,
        CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdAsync(query.ClaimId, ct);
        return claim is null
            ? Result.Failure<ClaimDto>(Error.NotFound(ErrorCodes.ClaimNotFound, "Claim", query.ClaimId))
            : Result.Success(claim.ToDto());
    }
}