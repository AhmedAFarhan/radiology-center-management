using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.UpdateReportTemplate;

public record UpdateReportTemplateCommand(
    Guid TemplateId,
    string Name,
    string Modality,
    string? BodyPart = null,
    string? Description = null,
    IReadOnlyList<ReportTemplateSectionInput>? Sections = null) : ICommand;