namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.ActivateReportTemplate;

public record ActivateReportTemplateCommand(Guid TemplateId) : ICommand;