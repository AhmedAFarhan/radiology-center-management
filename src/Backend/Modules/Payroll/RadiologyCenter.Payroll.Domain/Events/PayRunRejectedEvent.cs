using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Payroll.Domain.Events;

public sealed record PayRunRejectedEvent(Guid PayRunId, string? ProcessedBy) : DomainEvent;
