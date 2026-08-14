using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Catalog.Domain.Events;

public sealed record ExaminationTypeCreatedEvent(Guid ExaminationTypeId, string Code, string Name) : DomainEvent;
