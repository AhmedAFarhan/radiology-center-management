using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Domain.Results;

public enum ErrorKind
{
    None,
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
    Forbidden,
    LockedOut,
    Failure,
}

public sealed record Error(ErrorKind Kind, string Code, string Message)
{
    public static readonly Error None = new(ErrorKind.None, string.Empty, string.Empty);

    public static Error NotFound(string entity, object key) =>
        new(ErrorKind.NotFound, MessageCodes.Shared.KeyNotFound, $"{entity} with key '{key}' not found.");

    public static Error Validation(string code, string message) =>
        new(ErrorKind.Validation, code, message);

    public static Error Conflict(string message) =>
        new(ErrorKind.Conflict, "Conflict", message);

    public static Error Unauthorized(string message = "Unauthorized.") =>
        new(ErrorKind.Unauthorized, MessageCodes.Shared.Unauthorized, message);

    public static Error Forbidden(string message = "Forbidden.") =>
        new(ErrorKind.Forbidden, MessageCodes.Shared.Forbidden, message);

    public static Error LockedOut(string message = "Account is locked out.") =>
        new(ErrorKind.LockedOut, MessageCodes.Shared.LockedOut, message);

    public static Error Failure(string message) =>
        new(ErrorKind.Failure, "Failure", message);

    public static Error NotFound(string code, string entity, object key) =>
        new(ErrorKind.NotFound, code, $"{entity} with key '{key}' not found.");

    public static Error Conflict(string code, string message) =>
        new(ErrorKind.Conflict, code, message);

    public static Error Unauthorized(string code, string message) =>
        new(ErrorKind.Unauthorized, code, message);

    public static Error Forbidden(string code, string message) =>
        new(ErrorKind.Forbidden, code, message);

    public static Error LockedOut(string code, string message) =>
        new(ErrorKind.LockedOut, code, message);

    public static Error Failure(string code, string message) =>
        new(ErrorKind.Failure, code, message);
}
