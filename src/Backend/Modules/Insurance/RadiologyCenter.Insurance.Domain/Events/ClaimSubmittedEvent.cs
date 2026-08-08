using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimSubmittedEvent(
    Guid ClaimId,
    Guid ExaminationId,
    Guid PolicyId,
    decimal PayerShare,
    decimal PatientShare,
    decimal CopayApplied) : DomainEvent;