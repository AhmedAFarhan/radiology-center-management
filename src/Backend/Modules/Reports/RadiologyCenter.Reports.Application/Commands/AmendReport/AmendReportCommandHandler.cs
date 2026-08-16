using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.AmendReport;

public static class AmendReportCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        AmendReportCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        report.Amend(command.Reason);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(report.ToDto());
    }
}