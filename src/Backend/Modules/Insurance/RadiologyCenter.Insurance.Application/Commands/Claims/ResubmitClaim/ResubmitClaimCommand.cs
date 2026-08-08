namespace RadiologyCenter.Insurance.Application.Commands.Claims.ResubmitClaim;

public record ResubmitClaimCommand(Guid ClaimId) : ICommand;