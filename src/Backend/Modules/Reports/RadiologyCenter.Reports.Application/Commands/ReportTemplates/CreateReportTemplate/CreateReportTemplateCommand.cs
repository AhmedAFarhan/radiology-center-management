namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;

public record ReportTemplateSectionInput(
    string SectionType,
    string Title,
    string Body,
    int Position = 0,
    bool IsLocked = true);

public record CreateReportTemplateCommand(
    string Name,
    string Modality,
    string? BodyPart = null,
    string? Description = null,
    bool IsSystem = false,
    IReadOnlyList<ReportTemplateSectionInput>? Sections = null) : ICommand;