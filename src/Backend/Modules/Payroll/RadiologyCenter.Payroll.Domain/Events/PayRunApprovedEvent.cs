using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Payroll.Domain.Events;

public sealed record PayRunApprovedEvent(Guid PayRunId, string? ProcessedBy) : DomainEvent;
