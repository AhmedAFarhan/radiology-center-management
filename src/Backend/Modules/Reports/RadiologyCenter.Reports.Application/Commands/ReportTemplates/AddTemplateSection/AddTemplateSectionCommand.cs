using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.AddTemplateSection;

public record AddTemplateSectionCommand(
    Guid TemplateId,
    ReportTemplateSectionInput Section) : ICommand;