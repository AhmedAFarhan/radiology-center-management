using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationStartedEvent(Guid ExaminationId, Guid PerformedByUserId) : DomainEvent;
