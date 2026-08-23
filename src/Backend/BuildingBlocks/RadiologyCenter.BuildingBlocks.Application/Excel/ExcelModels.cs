namespace RadiologyCenter.BuildingBlocks.Application.Excel;

/// <summary>
/// Limits applied to every Excel import. Guards against oversized or
/// hostile workbooks while keeping typical master-data files comfortable.
/// </summary>
public static class ExcelImportLimits
{
    public const int MaxRows = 10_000;
    public const int MaxFileBytes = 5 * 1024 * 1024;
    public const string TemplateVersion = "1";
}

/// <summary>
/// A single data row parsed from an import workbook, with cell values keyed
/// by the header code declared in the module's column definitions.
/// </summary>
public sealed class ImportedRow
{
    public ImportedRow(int rowNumber, IReadOnlyDictionary<string, string?> cells)
    {
        RowNumber = rowNumber;
        Cells = cells;
    }

    /// <summary>1-based worksheet row number (header row is row 1).</summary>
    public int RowNumber { get; }

    public IReadOnlyDictionary<string, string?> Cells { get; }

    public string? Value(string headerCode)
        => Cells.TryGetValue(headerCode, out var value) ? value : null;
}

public sealed class ParsedWorkbook
{
    public ParsedWorkbook(string? templateVersion, IReadOnlyList<ImportedRow> rows)
    {
        TemplateVersion = templateVersion;
        Rows = rows;
    }

    /// <summary>Value of the hidden _TemplateVersion cell; null when absent.</summary>
    public string? TemplateVersion { get; }

    public IReadOnlyList<ImportedRow> Rows { get; }
}

/// <summary>A per-row failure. <see cref="Message"/> is already localized server-side.</summary>
public sealed record ExcelRowError(int RowNumber, string Code, string Message);

/// <summary>Outcome of a best-effort import: valid rows commit, failures are reported.</summary>
public sealed record ExcelImportResult(
    int TotalRows,
    int ImportedCount,
    IReadOnlyList<ExcelRowError> Errors)
{
    public bool HasErrors => Errors.Count > 0;

    public static readonly ExcelImportResult Empty = new(0, 0, Array.Empty<ExcelRowError>());
}
