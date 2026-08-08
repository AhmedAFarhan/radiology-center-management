using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Insurance.Domain.Events;

public sealed record ClaimPaidEvent(Guid ClaimId, Guid ExaminationId, decimal PaidAmount) : DomainEvent;