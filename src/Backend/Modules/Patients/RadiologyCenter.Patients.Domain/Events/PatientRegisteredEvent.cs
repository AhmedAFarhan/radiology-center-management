using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Patients.Domain.Events;

public sealed record PatientRegisteredEvent(Guid PatientId, string PatientCode, string FullName) : DomainEvent;
