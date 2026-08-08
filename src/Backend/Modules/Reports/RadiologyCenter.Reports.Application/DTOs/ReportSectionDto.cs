namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportSectionDto(
    Guid Id,
    string SectionType,
    string Title,
    string Body,
    int Position,
    bool IsLocked);
