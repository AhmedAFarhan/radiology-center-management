namespace RadiologyCenter.Reports.Application.Commands.ApplyReportTemplate;

public record ApplyReportTemplateCommand(Guid ReportId, Guid TemplateId) : ICommand;