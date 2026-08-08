namespace RadiologyCenter.Insurance.Application.Commands.Claims.AdjudicateClaim;

public enum ClaimAdjudication
{
    Approve,
    Reject
}

public record AdjudicateClaimCommand(
    Guid ClaimId,
    ClaimAdjudication Decision,
    decimal? ApprovedAmount = null,
    string? RejectionCode = null,
    string? RejectionReason = null) : ICommand;