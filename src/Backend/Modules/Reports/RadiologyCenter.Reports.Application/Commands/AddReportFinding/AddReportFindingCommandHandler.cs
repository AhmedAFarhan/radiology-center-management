using Mapster;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.AddReportFinding;

public static class AddReportFindingCommandHandler
{
    public static async Task<Result<ReportFindingDto>> HandleAsync(
        AddReportFindingCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportFindingDto>(Error.NotFound("Report", command.ReportId));

        var severity = FindingSeverity.FromName<FindingSeverity>(command.Severity);
        var finding = report.AddFinding(command.Region, command.Description, severity, command.Position);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(finding.Adapt<ReportFindingDto>());
    }
}