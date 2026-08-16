using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpdateReportFinding;

public static class UpdateReportFindingCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateReportFindingCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        var severity = FindingSeverity.FromName<FindingSeverity>(command.Severity);
        report.UpdateFinding(command.FindingId, command.Description, severity);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}