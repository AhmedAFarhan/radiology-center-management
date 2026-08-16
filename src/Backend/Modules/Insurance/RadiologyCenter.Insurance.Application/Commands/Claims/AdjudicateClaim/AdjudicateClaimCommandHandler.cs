using RadiologyCenter.Insurance.Application.Localization;
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
            return Result.Failure<ClaimDto>(Error.NotFound(ErrorCodes.ClaimNotFound, "Claim", command.ClaimId));

        switch (command.Decision)
        {
            case ClaimAdjudication.Approve:
                if (command.ApprovedAmount is null)
                    return Result.Failure<ClaimDto>(Error.Validation(ErrorCodes.ApprovedAmountRequired, "Approved amount is required when approving."));
                claim.AdjudicateApproved(command.ApprovedAmount.Value);
                break;
            case ClaimAdjudication.Reject:
                if (string.IsNullOrWhiteSpace(command.RejectionCode))
                    return Result.Failure<ClaimDto>(Error.Validation(ErrorCodes.RejectionCodeRequired, "Rejection code is required when rejecting."));
                if (string.IsNullOrWhiteSpace(command.RejectionReason))
                    return Result.Failure<ClaimDto>(Error.Validation(ErrorCodes.RejectionReasonRequired, "Rejection reason is required when rejecting."));

                var code = ClaimRejectionCode.FromName<ClaimRejectionCode>(command.RejectionCode);
                claim.AdjudicateRejected(code, command.RejectionReason);
                break;
            default:
                return Result.Failure<ClaimDto>(Error.Validation(ErrorCodes.UnsupportedDecision, "Unsupported decision."));
        }

        claimRepository.Update(claim);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(claim.ToDto());
    }
}
