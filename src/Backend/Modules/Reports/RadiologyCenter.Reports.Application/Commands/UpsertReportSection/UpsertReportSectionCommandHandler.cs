using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpsertReportSection;

public static class UpsertReportSectionCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        UpsertReportSectionCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

        var sectionType = ReportSectionType.FromName<ReportSectionType>(command.SectionType);

        report.UpsertSection(sectionType, command.Title, command.Body, command.Position, command.IsLocked);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(report.ToDto());
    }
}