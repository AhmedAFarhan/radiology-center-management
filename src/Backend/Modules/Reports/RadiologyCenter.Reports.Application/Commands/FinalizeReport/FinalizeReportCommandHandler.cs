using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.FinalizeReport;

public static class FinalizeReportCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        FinalizeReportCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        report.FinalizeReport();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(report.ToDto());
    }
}