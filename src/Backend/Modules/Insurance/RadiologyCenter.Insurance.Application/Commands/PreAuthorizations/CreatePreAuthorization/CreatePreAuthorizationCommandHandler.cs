using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.CreatePreAuthorization;

public static class CreatePreAuthorizationCommandHandler
{
    public static async Task<Result<PreAuthorizationDto>> HandleAsync(
        CreatePreAuthorizationCommand command,
        IInsurancePolicyRepository policyRepository,
        IPreAuthorizationRepository preAuthorizationRepository,
        IClaimRepository claimRepository,
        IInsuranceUnitOfWork unitOfWork,
        IInsuranceDirectory directory,
        CancellationToken ct)
    {
        if (await policyRepository.GetByIdAsync(command.PolicyId, ct) is not { } policy)
            return Result.Failure<PreAuthorizationDto>(Error.NotFound(ErrorCodes.PolicyNotFound, "Policy", command.PolicyId));
        if (!policy.IsActive)
            return Result.Failure<PreAuthorizationDto>(Error.Conflict(ErrorCodes.PolicyNotActive, "Policy is not active."));
        if (policy.PatientId != command.PatientId || await directory.GetExaminationPatientAsync(command.ExaminationId, ct) != command.PatientId)
            return Result.Failure<PreAuthorizationDto>(Error.Conflict("Insurance.ExaminationPatientMismatch", "The policy and examination must belong to the same patient."));

        if (await preAuthorizationRepository.GetByExaminationIdAsync(command.ExaminationId, ct) is not null)
            return Result.Failure<PreAuthorizationDto>(Error.Conflict(ErrorCodes.PreAuthorizationAlreadyExists, "A pre-authorization already exists for this examination."));
        if (await claimRepository.GetByExaminationIdAsync(command.ExaminationId, ct) is not null)
            return Result.Failure<PreAuthorizationDto>(Error.Conflict(ErrorCodes.ClaimAlreadyExists, "A claim already exists for this examination."));

        var preAuthorization = PreAuthorization.Create(
            command.ExaminationId,
            command.PatientId,
            command.PolicyId,
            command.EstimatedAmount,
            policy.IsGovernment);

        await preAuthorizationRepository.AddAsync(preAuthorization, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(preAuthorization.ToDto());
    }
}
