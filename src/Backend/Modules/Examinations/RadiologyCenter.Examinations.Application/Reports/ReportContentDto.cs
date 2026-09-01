namespace RadiologyCenter.Examinations.Application.Reports;

public sealed record ReportContentDto(
    byte[] Content,
    string FileName,
    string ContentType);
