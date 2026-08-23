namespace RadiologyCenter.Desktop.Models;

public sealed class ExcelImportResultDto
{
    public int TotalRows { get; set; }

    public int ImportedCount { get; set; }

    public IReadOnlyList<ExcelRowErrorDto> Errors { get; set; } = Array.Empty<ExcelRowErrorDto>();
}

public sealed class ExcelRowErrorDto
{
    public int RowNumber { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
