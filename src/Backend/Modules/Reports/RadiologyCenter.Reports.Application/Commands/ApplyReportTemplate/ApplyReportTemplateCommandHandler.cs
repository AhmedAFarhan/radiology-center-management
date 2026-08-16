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
        CancellationToken ct)
    {
        var report = await reportRepository.GetByIdWithVersionsAsync(command.ReportId, ct);
        if (report is null)
            return Result.Failure<ReportDto>(Error.NotFound(ErrorCodes.ReportNotFound, "Report", command.ReportId));

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

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(report.ToDto());
    }
}