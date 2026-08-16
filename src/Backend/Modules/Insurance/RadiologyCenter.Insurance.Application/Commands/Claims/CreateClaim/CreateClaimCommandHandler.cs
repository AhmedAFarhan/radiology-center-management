using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Services;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;

public static class CreateClaimCommandHandler
{
    public static async Task<Result<ClaimDto>> HandleAsync(
        CreateClaimCommand command,
        IInsurancePolicyRepository policyRepository,
        IClaimRepository claimRepository,
        IPreAuthorizationRepository preAuthorizationRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await policyRepository.GetByIdAsync(command.PolicyId, ct) is not { } policy)
            return Result.Failure<ClaimDto>(Error.NotFound(ErrorCodes.PolicyNotFound, "Policy", command.PolicyId));
        if (!policy.IsActive)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PolicyNotActive, "Policy is not active."));
        if (policy.PatientId != command.PatientId)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PolicyPatientMismatch, "The policy does not belong to the claim's patient."));

        if (await claimRepository.GetByExaminationIdAsync(command.ExaminationId, ct) is not null)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.ClaimAlreadyExists, "A claim already exists for this examination."));

        if (await preAuthorizationRepository.GetByIdAsync(command.PreAuthorizationId, ct) is not { } preAuth)
            return Result.Failure<ClaimDto>(Error.NotFound(ErrorCodes.PreAuthorizationNotFound, "PreAuthorization", command.PreAuthorizationId));
        if (preAuth.ExaminationId != command.ExaminationId)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PreAuthorizationExaminationMismatch, "Pre-authorization does not match the examination."));
        if (preAuth.PatientId != command.PatientId)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PreAuthorizationPatientMismatch, "Pre-authorization does not match the claim's patient."));
        if (preAuth.PolicyId != command.PolicyId)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PreAuthorizationPolicyMismatch, "Pre-authorization does not match the claim's policy."));
        if (preAuth.Status != PreAuthorizationStatus.Approved)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PreAuthorizationNotApproved, "Pre-authorization must be approved before creating a claim."));
        if (preAuth.ApprovedAmount is not { } approvedAmount)
            return Result.Failure<ClaimDto>(Error.Conflict(ErrorCodes.PreAuthorizationNoApprovedAmount, "Pre-authorization has no approved amount."));
        if (command.BilledAmount > approvedAmount)
            return Result.Failure<ClaimDto>(
                Error.Validation(
                    ErrorCodes.BilledAmountExceedsApproved,
                    $"Billed amount '{command.BilledAmount}' exceeds the pre-authorization approved amount of '{approvedAmount}'."));

        var split = CoverageCalculationService.Split(policy, command.BilledAmount);

        var claim = Claim.Create(
            command.ExaminationId,
            command.PatientId,
            command.PolicyId,
            command.PreAuthorizationId,
            command.BilledAmount,
            split.PayerShare,
            split.PatientShare);

        await claimRepository.AddAsync(claim, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(claim.ToDto());
    }
}