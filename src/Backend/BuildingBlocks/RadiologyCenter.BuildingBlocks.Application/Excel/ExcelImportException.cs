namespace RadiologyCenter.BuildingBlocks.Application.Excel;

/// <summary>
/// Raised when an uploaded file cannot be imported structurally. The code is
/// a localization key; args feed the "{0}" placeholders of the message.
/// </summary>
public sealed class ExcelImportException : Exception
{
    public ExcelImportException(string code, object[]? args = null)
        : base(code)
    {
        Code = code;
        Args = args ?? Array.Empty<object>();
    }

    public string Code { get; }

    public object[] Args { get; }
}
