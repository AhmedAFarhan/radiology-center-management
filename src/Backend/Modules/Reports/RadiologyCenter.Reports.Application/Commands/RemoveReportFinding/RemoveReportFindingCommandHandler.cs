using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Commands.RemoveReportFinding;

public static class RemoveReportFindingCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemoveReportFindingCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        report.RemoveFinding(command.FindingId);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}