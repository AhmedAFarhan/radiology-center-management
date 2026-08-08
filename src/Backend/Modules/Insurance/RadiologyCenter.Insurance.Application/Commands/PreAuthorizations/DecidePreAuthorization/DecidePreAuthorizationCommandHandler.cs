using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DecidePreAuthorization;

public static class DecidePreAuthorizationCommandHandler
{
    public static async Task<Result<PreAuthorizationDto>> HandleAsync(
        DecidePreAuthorizationCommand command,
        IPreAuthorizationRepository preAuthorizationRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var preAuthorization = await preAuthorizationRepository.GetByIdAsync(command.PreAuthorizationId, ct);
        if (preAuthorization is null)
            return Result.Failure<PreAuthorizationDto>(Error.NotFound("PreAuthorization", command.PreAuthorizationId));

        switch (command.Decision)
        {
            case PreAuthorizationDecision.Approve:
                if (command.ApprovedAmount is null)
                    return Result.Failure<PreAuthorizationDto>(Error.Validation("ApprovedAmount", "Approved amount is required when approving."));
                preAuthorization.Approve(command.ApprovedAmount.Value);
                break;
            case PreAuthorizationDecision.Deny:
                if (string.IsNullOrWhiteSpace(command.RejectionReason))
                    return Result.Failure<PreAuthorizationDto>(Error.Validation("RejectionReason", "Rejection reason is required when denying."));
                preAuthorization.Deny(command.RejectionReason);
                break;
            default:
                return Result.Failure<PreAuthorizationDto>(Error.Validation("Decision", "Unsupported decision."));
        }

        preAuthorizationRepository.Update(preAuthorization);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(preAuthorization.ToDto());
    }
}