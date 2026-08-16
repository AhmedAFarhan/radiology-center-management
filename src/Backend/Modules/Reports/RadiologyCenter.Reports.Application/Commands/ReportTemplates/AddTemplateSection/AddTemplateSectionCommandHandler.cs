using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.AddTemplateSection;

public static class AddTemplateSectionCommandHandler
{
    public static async Task<Result<ReportTemplateDto>> HandleAsync(
        AddTemplateSectionCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdWithSectionsAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure<ReportTemplateDto>(Error.NotFound(ErrorCodes.ReportTemplateNotFound, "ReportTemplate", command.TemplateId));

        if (template.IsSystem)
            return Result.Failure<ReportTemplateDto>(Error.Conflict(ErrorCodes.SystemTemplateReadOnly, "System templates cannot be modified."));

        var sectionType = ReportSectionType.FromName<ReportSectionType>(command.Section.SectionType);
        template.AddSection(sectionType, command.Section.Title, command.Section.Body, command.Section.Position, command.Section.IsLocked);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(template.ToDto());
    }
}