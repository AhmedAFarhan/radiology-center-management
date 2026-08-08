namespace RadiologyCenter.Reports.Application.Commands.RemoveReportFinding;

public record RemoveReportFindingCommand(Guid ReportId, Guid FindingId) : ICommand;