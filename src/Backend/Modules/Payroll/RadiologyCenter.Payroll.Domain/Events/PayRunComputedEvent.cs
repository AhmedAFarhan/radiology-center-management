using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Payroll.Domain.Events;

public sealed record PayRunComputedEvent(Guid PayRunId, string? ProcessedBy) : DomainEvent;
