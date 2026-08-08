namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportVersionDto(
    Guid Id,
    int VersionNumber,
    string? AmendmentReason,
    DateTime CreatedAt,
    IReadOnlyList<ReportSectionDto> Sections,
    IReadOnlyList<ReportFindingDto> Findings);
