namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeleteReportTemplate;

public record DeleteReportTemplateCommand(Guid TemplateId) : ICommand;