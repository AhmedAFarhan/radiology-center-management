namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DecidePreAuthorization;

public enum PreAuthorizationDecision
{
    Approve,
    Deny
}

public record DecidePreAuthorizationCommand(
    Guid PreAuthorizationId,
    PreAuthorizationDecision Decision,
    decimal? ApprovedAmount = null,
    string? RejectionReason = null) : ICommand;