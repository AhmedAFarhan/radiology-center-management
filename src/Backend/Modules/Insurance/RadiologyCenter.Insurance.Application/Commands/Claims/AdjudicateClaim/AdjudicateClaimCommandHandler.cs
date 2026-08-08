using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;

public static class AdjudicateClaimCommandHandler
{
    public static async Task<Result<ClaimDto>> HandleAsync(
        AdjudicateClaimCommand command,
        IClaimRepository claimRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdAsync(command.ClaimId, ct);
        if (claim is null)
            return Result.Failure<ClaimDto>(Error.NotFound("Claim", command.ClaimId));

        switch (command.Decision)
        {
            case ClaimAdjudication.Approve:
                if (command.ApprovedAmount is null)
                    return Result.Failure<ClaimDto>(Error.Validation("ApprovedAmount", "Approved amount is required when approving."));
                claim.AdjudicateApproved(command.ApprovedAmount.Value);
                break;
            case ClaimAdjudication.Reject:
                if (string.IsNullOrWhiteSpace(command.RejectionCode))
                    return Result.Failure<ClaimDto>(Error.Validation("RejectionCode", "Rejection code is required when rejecting."));
                if (string.IsNullOrWhiteSpace(command.RejectionReason))
                    return Result.Failure<ClaimDto>(Error.Validation("RejectionReason", "Rejection reason is required when rejecting."));

                var code = ClaimRejectionCode.FromName<ClaimRejectionCode>(command.RejectionCode);
                claim.AdjudicateRejected(code, command.RejectionReason);
                break;
            default:
                return Result.Failure<ClaimDto>(Error.Validation("Decision", "Unsupported decision."));
        }

        claimRepository.Update(claim);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(claim.ToDto());
    }
}