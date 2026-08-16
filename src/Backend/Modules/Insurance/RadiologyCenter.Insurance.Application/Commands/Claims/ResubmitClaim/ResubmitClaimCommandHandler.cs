using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.ResubmitClaim;

public static class ResubmitClaimCommandHandler
{
    public static async Task<Result<ClaimDto>> HandleAsync(
        ResubmitClaimCommand command,
        IClaimRepository claimRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdAsync(command.ClaimId, ct);
        if (claim is null)
            return Result.Failure<ClaimDto>(Error.NotFound(ErrorCodes.ClaimNotFound, "Claim", command.ClaimId));

        claim.Resubmit();

        claimRepository.Update(claim);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(claim.ToDto());
    }
}