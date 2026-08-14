using RadiologyCenter.Catalog.Domain.Enumerations;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.UpdateReportTemplate;

public static class UpdateReportTemplateCommandHandler
{
    public static async Task<Result<ReportTemplateDto>> HandleAsync(
        UpdateReportTemplateCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdWithSectionsAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure<ReportTemplateDto>(Error.NotFound("ReportTemplate", command.TemplateId));

        if (template.IsSystem)
            return Result.Failure<ReportTemplateDto>(Error.Conflict("System templates cannot be modified."));

        if (await templateRepository.ExistsByNameAsync(command.Name, template.Id, ct))
            return Result.Failure<ReportTemplateDto>(Error.Conflict($"A template named '{command.Name}' already exists."));

        var modality = Modality.FromName<Modality>(command.Modality);
        template.Update(command.Name, modality, command.BodyPart, command.Description);

        foreach (var sectionInput in command.Sections ?? [])
        {
            if (template.ContainsSection(ReportSectionType.FromName<ReportSectionType>(sectionInput.SectionType)))
                continue;

            template.AddSection(
                ReportSectionType.FromName<ReportSectionType>(sectionInput.SectionType),
                sectionInput.Title,
                sectionInput.Body,
                sectionInput.Position,
                sectionInput.IsLocked);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(template.ToDto());
    }
}