namespace RadiologyCenter.BuildingBlocks.Application.Excel;

/// <summary>
/// A file ready for download. Handlers return this wrapped in Result;
/// controllers translate it into a FileContentResult.
/// </summary>
public sealed record FileContentDto(
    byte[] Content,
    string FileName,
    string ContentType);

public static class ExcelContentTypes
{
    public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
