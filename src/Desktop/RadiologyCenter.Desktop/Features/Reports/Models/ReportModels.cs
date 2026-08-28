namespace RadiologyCenter.Desktop.Features.Reports.Models;

public sealed record ReportDto(
    string Id,
    string ExaminationId,
    string PatientId,
    string RadiologistId,
    string Status,
    string StatusKey,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    ReportVersionDto? CurrentVersion,
    string? PatientName = null,
    string? RadiologistName = null,
    string? ExaminationTypeName = null);

public sealed record ReportListItemDto(
    string Id,
    string ExaminationId,
    string PatientId,
    string RadiologistId,
    string Status,
    string StatusKey,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    string? PatientName = null,
    string? RadiologistName = null,
    string? ExaminationTypeName = null);

public sealed record ReportVersionDto(
    string Id,
    int VersionNumber,
    string? AmendmentReason,
    DateTime CreatedAt,
    IReadOnlyList<ReportSectionDto> Sections,
    IReadOnlyList<ReportFindingDto> Findings);

public sealed record ReportSectionDto(
    string Id,
    string SectionType,
    string Title,
    string Body,
    int Position,
    bool IsLocked,
    string SectionTypeKey = "");

public sealed record ReportFindingDto(
    string Id,
    string Region,
    string Description,
    string Severity,
    int Position,
    string SeverityKey = "");

public sealed record ReportTemplateDto(
    string Id,
    string Name,
    string Modality,
    string? BodyPart,
    string? Description,
    bool IsActive,
    bool IsSystem,
    int UseCount,
    IReadOnlyList<ReportTemplateSectionDto> Sections);

public sealed record ReportTemplateSectionDto(
    string Id,
    string SectionType,
    string Title,
    string Body,
    int Position,
    bool IsLocked,
    string SectionTypeKey = "");

public sealed class CreateReportDraftInput
{
    public string ExaminationId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string RadiologistId { get; set; } = string.Empty;
}

public sealed class UpsertReportSectionInput
{
    public string SectionType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsLocked { get; set; }
}

public sealed class AddReportFindingInput
{
    public string Region { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public int Position { get; set; }
}

public sealed class UpdateReportFindingInput
{
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}