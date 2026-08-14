using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Catalog.Domain.Events;

public sealed record ExaminationTypeUpdatedEvent(Guid ExaminationTypeId) : DomainEvent;
