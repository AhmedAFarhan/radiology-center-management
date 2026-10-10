using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using Mapster;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.AddReportFinding;

public static class AddReportFindingCommandHandler
{
    public static async Task<Result<ReportFindingDto>> HandleAsync(
        AddReportFindingCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, new[] { "reports:writing" }, ct);
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportFindingDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        if (command.ExpectedRevision is null)
            return Result.Failure<ReportFindingDto>(Error.Validation("Reports.RevisionRequired", "Reload the report in an updated workstation application before saving."));

        if (command.ExpectedRevision is int expected && expected != report.Revision)
            return Result.Failure<ReportFindingDto>(Error.Conflict("Reports.StaleRevision", "This report was changed on another workstation. Reload it before saving."));

        var severity = FindingSeverity.FromName<FindingSeverity>(command.Severity);
        var finding = report.AddFinding(command.Region, command.Description, severity, command.Position);

        report.MarkChanged();
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success(finding.Adapt<ReportFindingDto>());
    }
}
