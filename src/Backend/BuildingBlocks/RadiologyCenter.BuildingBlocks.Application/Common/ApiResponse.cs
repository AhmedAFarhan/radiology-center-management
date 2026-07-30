using System.Text.Json.Serialization;
using RadiologyCenter.BuildingBlocks.Domain.Results;

namespace RadiologyCenter.BuildingBlocks.Application.Common;

public class ApiResponse
{
    [JsonPropertyOrder(0)]
    public bool Success { get; init; }

    [JsonPropertyOrder(1)]
    public string? Message { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(2)]
    public object? Data { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(3)]
    public ApiError? Error { get; init; }

    public static ApiResponse Ok(object? data = null, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
    };

    public static ApiResponse Fail(string message, ApiError? error = null) => new()
    {
        Success = false,
        Message = message,
        Error = error,
    };

    public static ApiResponse FromResult<T>(Result<T> result, string? successMessage = null) =>
        result.IsSuccess ? Ok(result.Value, successMessage) : Fail(result.Error!.Message, ApiError.FromError(result.Error));

    public static ApiResponse FromResult(Result result, string? successMessage = null) =>
        result.IsSuccess ? Ok(null, successMessage) : Fail(result.Error!.Message, ApiError.FromError(result.Error));
}

public class ApiResponse<T>
{
    [JsonPropertyOrder(0)]
    public bool Success { get; init; }

    [JsonPropertyOrder(1)]
    public string? Message { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(2)]
    public T? Data { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(3)]
    public ApiError? Error { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
    };

    public static ApiResponse<T> Fail(string message, ApiError? error = null) => new()
    {
        Success = false,
        Message = message,
        Error = error,
    };
}

public class ApiError
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; init; }

    public string Message { get; init; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Details { get; init; }

    public static ApiError FromError(Error error) => new()
    {
        Code = error.Code,
        Message = error.Message,
    };

    public static ApiError FromException(Exception ex, string? code = null) => new()
    {
        Code = code ?? "Error",
        Message = ex.Message,
    };
}
