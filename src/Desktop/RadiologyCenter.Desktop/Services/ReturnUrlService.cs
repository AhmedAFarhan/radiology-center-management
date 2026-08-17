namespace RadiologyCenter.Desktop.Services;

public sealed class ReturnUrlService
{
    private string? _returnUrl;

    public void Store(string url) => _returnUrl = url;

    public string? Consume()
    {
        var url = _returnUrl;
        _returnUrl = null;
        return url;
    }
}