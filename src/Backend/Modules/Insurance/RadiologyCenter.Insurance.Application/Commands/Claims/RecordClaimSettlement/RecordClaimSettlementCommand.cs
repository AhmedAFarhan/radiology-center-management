namespace RadiologyCenter.Insurance.Application.Commands.Claims.RecordClaimSettlement;

public record RecordClaimSettlementCommand(
    Guid ClaimId,
    string Method,
    decimal Amount,
    string? Reference = null) : ICommand;