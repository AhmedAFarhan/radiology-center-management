using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationCompletedEvent(Guid VisitId, Guid ExaminationId) : DomainEvent;
