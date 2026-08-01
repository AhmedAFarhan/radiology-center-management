using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationCreatedEvent(Guid ExaminationId, Guid ExaminationTypeId) : DomainEvent;
