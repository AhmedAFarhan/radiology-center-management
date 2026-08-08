using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimApprovedEvent(Guid ClaimId, Guid ExaminationId, Guid PolicyId, decimal ApprovedAmount) : DomainEvent;