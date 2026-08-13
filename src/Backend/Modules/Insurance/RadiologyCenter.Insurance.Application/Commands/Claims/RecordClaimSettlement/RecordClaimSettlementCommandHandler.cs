using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;

public static class RecordClaimSettlementCommandHandler
{
    public static async Task<Result<ClaimDto>> HandleAsync(
        RecordClaimSettlementCommand command,
        IClaimRepository claimRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var claim = await claimRepository.GetByIdForUpdateAsync(command.ClaimId, ct);
        if (claim is null)
            return Result.Failure<ClaimDto>(Error.NotFound("Claim", command.ClaimId));

        if (command.Amount > claim.RemainingOwed)
            return Result.Failure<ClaimDto>(Error.Validation(
                "SettlementExceedsRemaining",
                $"Settlement of {command.Amount} exceeds the remaining {claim.RemainingOwed} owed for claim '{claim.Id}'."));

        var method = SettlementMethod.FromName<SettlementMethod>(command.Method);

        claim.RecordSettlement(method, command.Amount, command.Reference);

        claimRepository.Update(claim);
        await transaction.CommitAsync(ct);

        return Result.Success(claim.ToDto());
    }
}