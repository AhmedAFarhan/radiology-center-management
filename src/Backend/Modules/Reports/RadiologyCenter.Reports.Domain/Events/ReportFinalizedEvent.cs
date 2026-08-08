using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Reports.Domain.Events;

public sealed record ReportFinalizedEvent(Guid ReportId, int VersionNumber) : DomainEvent;