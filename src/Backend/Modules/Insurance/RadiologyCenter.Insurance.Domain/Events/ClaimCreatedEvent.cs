using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimCreatedEvent(Guid ClaimId, Guid ExaminationId, Guid PolicyId) : DomainEvent;