namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.RemoveTemplateSection;

public record RemoveTemplateSectionCommand(Guid TemplateId, Guid SectionId) : ICommand;