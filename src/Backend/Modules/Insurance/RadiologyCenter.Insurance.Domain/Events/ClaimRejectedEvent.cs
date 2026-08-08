using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimRejectedEvent(Guid ClaimId, Guid ExaminationId, int RejectionCode, string Reason) : DomainEvent;