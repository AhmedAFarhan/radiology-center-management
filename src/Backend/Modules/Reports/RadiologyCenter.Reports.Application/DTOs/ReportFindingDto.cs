namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportFindingDto(
    Guid Id,
    string Region,
    string Description,
    string Severity,
    int Position);
