namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportTemplateDto(
    Guid Id,
    string Name,
    string Modality,
    string? BodyPart,
    string? Description,
    bool IsActive,
    bool IsSystem,
    int UseCount,
    IReadOnlyList<ReportTemplateSectionDto> Sections);
