using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Reports.Domain.Events;

public sealed record ReportAmendedEvent(Guid ReportId, int NewVersionNumber, string Reason) : DomainEvent;