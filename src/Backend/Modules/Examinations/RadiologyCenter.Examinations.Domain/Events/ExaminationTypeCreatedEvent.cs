using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationTypeCreatedEvent(Guid ExaminationTypeId, string Code, string Name) : DomainEvent;
