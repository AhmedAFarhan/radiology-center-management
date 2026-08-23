namespace RadiologyCenter.BuildingBlocks.Application.Excel;

/// <summary>
/// Builds localized .xlsx exports and import templates from declarative
/// column definitions. Implemented over ClosedXML in the Infrastructure layer.
/// </summary>
public interface IExcelService
{
    /// <summary>Exports rows into a styled workbook with localized headers.</summary>
    byte[] Export<T>(
        string sheetName,
        string fileName,
        IReadOnlyList<ExcelColumn<T>> columns,
        IEnumerable<T> rows);

    /// <summary>
    /// Builds an empty import template: localized header row, optional
    /// sample row, a visible Instructions sheet, reference sheets of
    /// allowed enum values, and a hidden _TemplateVersion cell used to
    /// reject stale files.
    /// </summary>
    byte[] CreateTemplate(
        string sheetName,
        IReadOnlyList<(string HeaderCode, string HeaderFallback)> columns,
        IReadOnlyList<object?>? sampleRow = null,
        IReadOnlyList<(string SheetName, IReadOnlyList<string> Values)>? referenceSheets = null,
        IReadOnlyList<(string TextCode, string TextFallback)>? instructions = null);

    /// <summary>Parses and validates an uploaded template workbook.</summary>
    ParsedWorkbook ReadTemplate(
        Stream stream,
        IReadOnlyList<string> expectedHeaderCodes);
}
