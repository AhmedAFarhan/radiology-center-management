namespace RadiologyCenter.Desktop.Models;

public sealed class ApiEnvelope<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
}

public sealed class ApiEnvelope
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ApiError? Error { get; set; }
}

public sealed class ApiError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public object? Details { get; set; }
}

public sealed record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt,
    bool MustChangePassword = false);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);

public sealed record EnumOptionDto(
    string Key,
    string Value);
