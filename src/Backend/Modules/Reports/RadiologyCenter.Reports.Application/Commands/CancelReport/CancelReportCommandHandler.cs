using RadiologyCenter.Reports.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Commands.CancelReport;

public static class CancelReportCommandHandler
{
    public static async Task<Result> HandleAsync(
        CancelReportCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure(Error.NotFound("Report", command.ReportId));

        report.Cancel(command.Reason);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}