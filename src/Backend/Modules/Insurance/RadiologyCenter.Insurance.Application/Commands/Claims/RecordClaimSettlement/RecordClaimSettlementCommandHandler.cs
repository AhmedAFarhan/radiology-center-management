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
        var claim = await claimRepository.GetByIdAsync(command.ClaimId, ct);
        if (claim is null)
            return Result.Failure<ClaimDto>(Error.NotFound("Claim", command.ClaimId));

        var method = SettlementMethod.FromName<SettlementMethod>(command.Method);

        claim.RecordSettlement(method, command.Amount, command.Reference);

        claimRepository.Update(claim);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(claim.ToDto());
    }
}