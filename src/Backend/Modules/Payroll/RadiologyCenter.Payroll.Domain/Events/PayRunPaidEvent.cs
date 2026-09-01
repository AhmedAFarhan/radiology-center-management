using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Payroll.Domain.Events;

public sealed record PayRunPaidEvent(Guid PayRunId, string? ProcessedBy) : DomainEvent;
