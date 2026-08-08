using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Reports.Domain.Events;

public sealed record ReportCanceledEvent(Guid ReportId) : DomainEvent;