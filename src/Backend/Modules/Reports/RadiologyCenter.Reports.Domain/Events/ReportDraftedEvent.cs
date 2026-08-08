using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Reports.Domain.Events;

public sealed record ReportDraftedEvent(Guid ReportId, Guid ExaminationId) : DomainEvent;