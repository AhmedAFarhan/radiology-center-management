using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Patients.Domain.Events;

public sealed record PatientUpdatedEvent(Guid PatientId, string PatientCode) : DomainEvent;
