namespace RadiologyCenter.Reports.Application.Commands.AddReportFinding;

public record AddReportFindingCommand(
    Guid ReportId,
    string Region,
    string Description,
    string Severity,
    int Position = 0) : ICommand;