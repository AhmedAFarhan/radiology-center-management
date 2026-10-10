namespace RadiologyCenter.Reports.Application.Commands.FinalizeReport;

public record FinalizeReportCommand(Guid ReportId) : ICommand
{
    public int? ExpectedRevision { get; init; }
}