namespace RadiologyCenter.Reports.Application.Commands.CancelReport;

public record CancelReportCommand(Guid ReportId, string? Reason = null) : ICommand;