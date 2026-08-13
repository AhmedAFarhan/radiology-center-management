namespace RadiologyCenter.BuildingBlocks.Domain.Results;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string entity, object key) =>
        new("NotFound", $"{entity} with key '{key}' not found.");

    public static Error Validation(string code, string message) =>
        new(code, message);

    public static Error Conflict(string message) =>
        new("Conflict", message);

    public static Error Unauthorized(string message = "Unauthorized.") =>
        new("Unauthorized", message);

    public static Error Forbidden(string message = "Forbidden.") =>
        new("Forbidden", message);

    public static Error LockedOut(string message = "Account is locked out.") =>
        new("LockedOut", message);

    public static Error Failure(string message) =>
        new("Failure", message);
}
