using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.ApplyReportTemplate;

public static class ApplyReportTemplateCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        ApplyReportTemplateCommand command,
        IReportRepository reportRepository,
        IReportTemplateRepository templateRepository,
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

        var template = await templateRepository.GetByIdWithSectionsAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportTemplateNotFound, "ReportTemplate", command.TemplateId));

        foreach (var section in template.Sections.OrderBy(s => s.Position))
        {
            report.UpsertSection(
                section.SectionType,
                section.Title,
                section.Body,
                section.Position,
                section.IsLocked);
        }

        template.RegisterUse();

        report.MarkChanged();
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success(report.ToDto());
    }
}