using RadiologyCenter.Insurance.Application.Abstractions;
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
            return Result.Failure<ClaimDto>(Error.NotFound("Policy", command.PolicyId));
        if (!policy.IsActive)
            return Result.Failure<ClaimDto>(Error.Conflict("Policy is not active."));

        if (await claimRepository.GetByExaminationIdAsync(command.ExaminationId, ct) is not null)
            return Result.Failure<ClaimDto>(Error.Conflict("A claim already exists for this examination."));

        if (await preAuthorizationRepository.GetByIdAsync(command.PreAuthorizationId, ct) is not { } preAuth)
            return Result.Failure<ClaimDto>(Error.NotFound("PreAuthorization", command.PreAuthorizationId));
        if (preAuth.ExaminationId != command.ExaminationId)
            return Result.Failure<ClaimDto>(Error.Conflict("Pre-authorization does not match the examination."));
        if (preAuth.Status != PreAuthorizationStatus.Approved)
            return Result.Failure<ClaimDto>(Error.Conflict("Pre-authorization must be approved before creating a claim."));

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