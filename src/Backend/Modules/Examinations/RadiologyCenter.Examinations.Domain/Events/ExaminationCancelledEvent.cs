using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationCancelledEvent(Guid VisitId, Guid ExaminationId) : DomainEvent;
