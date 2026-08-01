using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationTypeUpdatedEvent(Guid ExaminationTypeId) : DomainEvent;
