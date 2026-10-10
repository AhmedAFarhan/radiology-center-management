using RadiologyCenter.BuildingBlocks.Application.Abstractions;
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
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, new[] { "reports:writing" }, ct);
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        if (command.ExpectedRevision is null)
            return Result.Failure(Error.Validation("Reports.RevisionRequired", "Reload the report in an updated workstation application before saving."));

        if (command.ExpectedRevision is int expected && expected != report.Revision)
            return Result.Failure(Error.Conflict("Reports.StaleRevision", "This report was changed on another workstation. Reload it before saving."));

        var severity = FindingSeverity.FromName<FindingSeverity>(command.Severity);
        if (!report.CurrentVersion.Findings.Any(f => f.Id == command.FindingId))
            return Result.Failure(Error.NotFound("Reports.FindingNotFound", "Finding", command.FindingId));
        report.UpdateFinding(command.FindingId, command.Description, severity);

        report.MarkChanged();
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}
