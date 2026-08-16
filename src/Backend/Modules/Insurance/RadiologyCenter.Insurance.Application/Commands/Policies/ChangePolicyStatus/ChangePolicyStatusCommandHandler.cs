using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.ChangePolicyStatus;

public static class ChangePolicyStatusCommandHandler
{
    public static async Task<Result<InsurancePolicyDto>> HandleAsync(
        ChangePolicyStatusCommand command,
        IInsurancePolicyRepository policyRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var policy = await policyRepository.GetByIdAsync(command.PolicyId, ct);
        if (policy is null)
            return Result.Failure<InsurancePolicyDto>(Error.NotFound(ErrorCodes.PolicyNotFound, "Policy", command.PolicyId));

        switch (command.Action)
        {
            case PolicyAction.Deactivate:
                policy.Deactivate();
                break;
            case PolicyAction.Reactivate:
                policy.Reactivate();
                break;
            case PolicyAction.Expire:
                policy.MarkExpired();
                break;
            default:
                return Result.Failure<InsurancePolicyDto>(Error.Validation(ErrorCodes.UnsupportedPolicyAction, "Unsupported policy action."));
        }

        policyRepository.Update(policy);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(policy.ToDto());
    }
}