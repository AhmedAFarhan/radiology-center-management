namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportTemplateSectionDto(
    Guid Id,
    string SectionType,
    string Title,
    string Body,
    int Position,
    bool IsLocked);
