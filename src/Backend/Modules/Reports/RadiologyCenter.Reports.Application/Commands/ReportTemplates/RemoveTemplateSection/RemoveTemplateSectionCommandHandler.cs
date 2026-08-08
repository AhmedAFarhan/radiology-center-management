using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.RemoveTemplateSection;

public static class RemoveTemplateSectionCommandHandler
{
    public static async Task<Result<ReportTemplateDto>> HandleAsync(
        RemoveTemplateSectionCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdWithSectionsAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure<ReportTemplateDto>(Error.NotFound("ReportTemplate", command.TemplateId));

        template.RemoveSection(command.SectionId);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(template.ToDto());
    }
}