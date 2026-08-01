using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record VisitCreatedEvent(Guid VisitId, Guid PatientId) : DomainEvent;
