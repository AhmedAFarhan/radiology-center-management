namespace RadiologyCenter.BuildingBlocks.Application.Excel;

/// <summary>
/// Rendering style applied to an exported column's cells.
/// </summary>
public enum ExcelColumnType
{
    Text,
    Number,
    Currency,
    Date,
}

/// <summary>
/// Declarative definition of one exported column. <paramref name="HeaderCode"/>
/// is resolved through the shared translator (module resource "codes" sections)
/// with <paramref name="HeaderFallback"/> as the English fallback.
/// </summary>
public sealed class ExcelColumn<T>
{
    public ExcelColumn(
        string headerCode,
        string headerFallback,
        Func<T, object?> selector,
        ExcelColumnType columnType = ExcelColumnType.Text,
        double width = 22)
    {
        HeaderCode = headerCode;
        HeaderFallback = headerFallback;
        Selector = selector;
        ColumnType = columnType;
        Width = width;
    }

    public string HeaderCode { get; }
    public string HeaderFallback { get; }
    public Func<T, object?> Selector { get; }
    public ExcelColumnType ColumnType { get; }
    public double Width { get; }
}
