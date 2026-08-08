using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;

public static class CreateReportTemplateCommandHandler
{
    public static async Task<Result<ReportTemplateDto>> HandleAsync(
        CreateReportTemplateCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await templateRepository.ExistsByNameAsync(command.Name, ct: ct))
            return Result.Failure<ReportTemplateDto>(Error.Conflict($"A template named '{command.Name}' already exists."));

        var modality = Modality.FromName<Modality>(command.Modality);
        var template = ReportTemplate.Create(command.Name, modality, command.BodyPart, command.Description, command.IsSystem);

        if (command.Sections is not null)
        {
            foreach (var sectionInput in command.Sections.OrderBy(s => s.Position))
            {
                var sectionType = ReportSectionType.FromName<ReportSectionType>(sectionInput.SectionType);
                template.AddSection(sectionType, sectionInput.Title, sectionInput.Body, sectionInput.Position, sectionInput.IsLocked);
            }
        }

        await templateRepository.AddAsync(template, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(template.ToDto());
    }
}