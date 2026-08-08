namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeactivateReportTemplate;

public record DeactivateReportTemplateCommand(Guid TemplateId) : ICommand;