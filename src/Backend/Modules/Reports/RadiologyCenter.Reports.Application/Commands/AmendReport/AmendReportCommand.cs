namespace RadiologyCenter.Reports.Application.Commands.AmendReport;

public record AmendReportCommand(Guid ReportId, string Reason) : ICommand
{
    public int? ExpectedRevision { get; init; }
}