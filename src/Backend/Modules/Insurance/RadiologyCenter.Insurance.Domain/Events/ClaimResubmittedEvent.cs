using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimResubmittedEvent(Guid ClaimId, Guid ExaminationId, int RejectionCount) : DomainEvent;