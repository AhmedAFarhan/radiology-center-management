using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimSettledEvent(
    Guid ClaimId,
    Guid ExaminationId,
    decimal Amount,
    decimal TotalSettled,
    decimal RemainingOwed) : DomainEvent;