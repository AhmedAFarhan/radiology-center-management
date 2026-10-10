using RadiologyCenter.BuildingBlocks.Application.Abstractions;
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
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, new[] { "reports:writing" }, ct);
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        if (command.ExpectedRevision is null)
            return Result.Failure<ReportDto>(Error.Validation("Reports.RevisionRequired", "Reload the report in an updated workstation application before saving."));

        if (command.ExpectedRevision is int expected && expected != report.Revision)
            return Result.Failure<ReportDto>(Error.Conflict("Reports.StaleRevision", "This report was changed on another workstation. Reload it before saving."));

        report.Amend(command.Reason);

        report.MarkChanged();
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success(report.ToDto());
    }
}