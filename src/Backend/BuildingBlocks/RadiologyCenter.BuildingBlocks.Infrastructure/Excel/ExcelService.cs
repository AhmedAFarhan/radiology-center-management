using System.Text;
using ClosedXML.Excel;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Excel;

/// <summary>
/// ClosedXML implementation of the shared Excel engine. Headers and error
/// messages are resolved through the ambient translator so exports match the
/// caller's culture.
/// </summary>
public sealed class ExcelService : IExcelService
{
    public byte[] Export<T>(
        string sheetName,
        string fileName,
        IReadOnlyList<ExcelColumn<T>> columns,
        IEnumerable<T> rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(sheetName);

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];
            var cell = sheet.Cell(1, c + 1);
            cell.Value = Translator.LocalizeCode(column.HeaderCode, column.HeaderFallback);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(ExcelTheme.HeaderBackground);
            sheet.Column(c + 1).Width = column.Width;
        }

        var rowIndex = 2;
        foreach (var row in rows)
        {
            for (var c = 0; c < columns.Count; c++)
            {
                WriteCell(sheet.Cell(rowIndex, c + 1), columns[c], row);
            }
            rowIndex++;
        }

        sheet.SheetView.FreezeRows(1);
        sheet.Row(1).Height = 20;

        return Save(workbook, fileName);
    }

    public byte[] CreateTemplate(
        string sheetName,
        IReadOnlyList<(string HeaderCode, string HeaderFallback)> columns,
        IReadOnlyList<object?>? sampleRow = null,
        IReadOnlyList<(string SheetName, IReadOnlyList<string> Values)>? referenceSheets = null,
        IReadOnlyList<(string TextCode, string TextFallback)>? instructions = null)
    {
        using var workbook = new XLWorkbook();

        if (instructions is { Count: > 0 })
        {
            var help = workbook.Worksheets.Add("Instructions");
            help.Cell(1, 1).Value = Translator.LocalizeCode("Excel.Instructions.Title", "How to fill this template");
            help.Cell(1, 1).Style.Font.Bold = true;
            help.Cell(1, 1).Style.Font.FontSize = 13;
            help.Row(1).Height = 24;

            var row = 2;
            foreach (var (code, fallback) in instructions)
            {
                help.Cell(row, 1).Value = Translator.LocalizeCode(code, fallback);
                row++;
            }

            help.Column(1).Width = 100;
        }

        var sheet = workbook.Worksheets.Add(sheetName);

        for (var c = 0; c < columns.Count; c++)
        {
            var cell = sheet.Cell(1, c + 1);
            cell.Value = Translator.LocalizeCode(columns[c].HeaderCode, columns[c].HeaderFallback);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(ExcelTheme.HeaderBackground);
            sheet.Column(c + 1).Width = 24;
        }

        if (sampleRow is { Count: > 0 })
        {
            for (var c = 0; c < sampleRow.Count && c < columns.Count; c++)
                sheet.Cell(2, c + 1).Value = ToXlValue(sampleRow[c]);

            var note = sheet.Range(2, 1, 2, Math.Max(columns.Count, 1));
            note.Style.Font.Italic = true;
            note.Style.Font.FontColor = XLColor.FromHtml(ExcelTheme.NoteTextColor);
        }

        if (referenceSheets is not null)
        {
            foreach (var (name, values) in referenceSheets)
            {
                var refSheet = workbook.Worksheets.Add(name);
                for (var i = 0; i < values.Count; i++)
                    refSheet.Cell(i + 1, 1).Value = values[i];
                refSheet.Columns().AdjustToContents();
                refSheet.SetTabColor(XLColor.FromHtml(ExcelTheme.ReferenceTabColor));
            }

            foreach (var (name, values) in referenceSheets)
            {
                var columnIndex = GetColumnIndexForReferenceSheet(columns, name);
                if (columnIndex == 0 || values.Count == 0)
                    continue;

                var target = sheet.Range(2, columnIndex, ExcelImportLimits.MaxRows + 1, columnIndex);
                var validation = target.CreateDataValidation();
                validation.List(workbook.Worksheet(name).Range(1, 1, values.Count, 1), true);
            }
        }

        var meta = workbook.Worksheets.Add("_Meta");
        meta.Cell(1, 1).Value = "_TemplateVersion";
        meta.Cell(1, 2).Value = ExcelImportLimits.TemplateVersion;
        meta.Hide();

        return Save(workbook, sheetName);
    }

    public ParsedWorkbook ReadTemplate(
        Stream stream,
        IReadOnlyList<string> expectedHeaderCodes)
    {
        using var workbook = Load(stream);
        var sheet = workbook.Worksheets.FirstOrDefault(ws =>
                ws.Visibility == XLWorksheetVisibility.Visible
                && !ws.Name.Equals("Instructions", StringComparison.OrdinalIgnoreCase))
            ?? throw new ExcelImportException("Excel.EmptyFile");

        var localizedHeaders = expectedHeaderCodes
            .Select(code => (Code: code, Text: Normalize(Translator.LocalizeCode(code, code))))
            .ToList();

        var headerMap = new Dictionary<string, int>();
        var usedColumns = new HashSet<int>();
        var width = Math.Min(sheet.LastColumnUsed()?.ColumnNumber() ?? 0, 256);

        for (var c = 1; c <= width; c++)
        {
            var text = Normalize(sheet.Cell(1, c).GetString());
            if (text.Length == 0)
                continue;

            var match = localizedHeaders.FirstOrDefault(h => h.Text == text || Normalize(h.Code) == text);
            if (match.Code is not null && !usedColumns.Contains(c))
            {
                headerMap[match.Code] = c;
                usedColumns.Add(c);
            }
        }

        var missing = localizedHeaders.Select(h => h.Code).Where(code => !headerMap.ContainsKey(code)).ToList();
        if (missing.Count > 0)
            throw new ExcelImportException("Excel.MissingColumns", [string.Join(", ", missing)]);

        string? version = null;
        if (workbook.TryGetWorksheet("_Meta", out var meta))
            version = meta.Cell(1, 2).GetString();

        var rows = new List<ImportedRow>();
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

        if (lastRow - 1 > ExcelImportLimits.MaxRows)
            throw new ExcelImportException("Excel.TooManyRows", [ExcelImportLimits.MaxRows]);

        for (var r = 2; r <= lastRow; r++)
        {
            var cells = new Dictionary<string, string?>(StringComparer.Ordinal);
            var hasValue = false;

            foreach (var (code, columnIndex) in headerMap)
            {
                var value = ReadCellText(sheet.Cell(r, columnIndex));
                cells[code] = value;
                if (!string.IsNullOrWhiteSpace(value))
                    hasValue = true;
            }

            if (hasValue)
                rows.Add(new ImportedRow(r, cells));
        }

        return new ParsedWorkbook(version, rows);
    }

    private static int GetColumnIndexForReferenceSheet(
        IReadOnlyList<(string HeaderCode, string HeaderFallback)> columns,
        string sheetName)
    {
        for (var i = 0; i < columns.Count; i++)
        {
            var (code, fallback) = columns[i];
            if (fallback.Equals(sheetName, StringComparison.OrdinalIgnoreCase)
                || code.EndsWith("." + sheetName, StringComparison.OrdinalIgnoreCase))
                return i + 1;
        }

        return 0;
    }

    private static void WriteCell<T>(IXLCell cell, ExcelColumn<T> column, T row)
    {
        switch (column.ColumnType)
        {
            case ExcelColumnType.Number:
            {
                var raw = column.Selector(row);
                if (raw is null)
                    return;
                if (raw is IConvertible convertible)
                    cell.Value = convertible.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                else
                    cell.Value = ToSanitizedText(raw.ToString());
                break;
            }
            case ExcelColumnType.Currency:
            {
                var raw = column.Selector(row);
                if (raw is decimal dec)
                    cell.Value = (double)dec;
                else if (raw is IConvertible convertible)
                    cell.Value = convertible.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
            }
            case ExcelColumnType.Date:
            {
                if (column.Selector(row) is DateTime date)
                {
                    cell.Value = date;
                    cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                }
                break;
            }
            default:
                cell.Value = ToSanitizedText(column.Selector(row)?.ToString());
                break;
        }
    }

    private static XLCellValue ToXlValue(object? value) => value switch
    {
        null => Blank.Value,
        string s => ToSanitizedText(s),
        _ => value.ToString() ?? string.Empty,
    };

    /// <summary>Neutralizes formula injection (=, +, -, @ prefixes) on text cells.</summary>
    private static string ToSanitizedText(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value[0] is '=' or '+' or '@' or '\t' or '\r'
            ? $"'{value}"
            : value.StartsWith('-') && value.Length > 1 && !double.TryParse(value, out _)
                ? $"'{value}"
                : value;
    }

    private static string? ReadCellText(IXLCell cell)
    {
        if (cell.DataType == XLDataType.Error || cell.DataType == XLDataType.Blank)
            return null;

        return cell.DataType switch
        {
            XLDataType.DateTime => cell.GetDateTime().ToString("yyyy-MM-dd"),
            XLDataType.TimeSpan => cell.GetTimeSpan().ToString(@"hh\:mm"),
            XLDataType.Boolean => cell.GetBoolean() ? "true" : "false",
            XLDataType.Number => IsIntegral(cell.GetValue<double>())
                ? ((long)cell.GetValue<double>()).ToString(System.Globalization.CultureInfo.InvariantCulture)
                : cell.GetValue<double>().ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => TrimFormulaPrefix(cell.GetString()),
        };
    }

    private static bool IsIntegral(double value) => Math.Abs(value % 1) < double.Epsilon;

    private static string TrimFormulaPrefix(string value)
    {
        if (value.StartsWith('\''))
            value = value[1..];

        return value.Trim();
    }

    private static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var ch in value.Trim().ToLowerInvariant())
            builder.Append(char.IsWhiteSpace(ch) ? ' ' : ch);

        return builder.ToString();
    }

    private static XLWorkbook Load(Stream stream)
    {
        try
        {
            return new XLWorkbook(stream);
        }
        catch (Exception)
        {
            throw new ExcelImportException("Excel.InvalidFile");
        }
    }

    private static byte[] Save(XLWorkbook workbook, string fileName)
    {
        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }
}
