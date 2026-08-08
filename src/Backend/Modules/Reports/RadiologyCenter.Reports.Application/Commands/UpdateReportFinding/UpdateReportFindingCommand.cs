namespace RadiologyCenter.Reports.Application.Commands.UpdateReportFinding;

public record UpdateReportFindingCommand(
    Guid ReportId,
    Guid FindingId,
    string Description,
    string Severity) : ICommand;