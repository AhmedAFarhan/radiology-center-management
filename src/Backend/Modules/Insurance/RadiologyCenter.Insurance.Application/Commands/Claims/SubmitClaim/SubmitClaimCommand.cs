namespace RadiologyCenter.Insurance.Application.Commands.Claims.SubmitClaim;

public record SubmitClaimCommand(Guid ClaimId) : ICommand;