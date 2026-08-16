namespace RadiologyCenter.Identity.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string PasswordDifferent = "Identity.PasswordDifferent";
    public const string RoleNameExists = "Identity.RoleNameExists";
    public const string AtLeastOneRole = "Identity.AtLeastOneRole";
    public const string RoleIdsNotEmpty = "Identity.RoleIdsNotEmpty";
    public const string EmailRegistered = "Identity.EmailRegistered";
    public const string UsernameTaken = "Identity.UsernameTaken";
    public const string UserNotFound = "Identity.UserNotFound";
    public const string RoleNotFound = "Identity.RoleNotFound";
    public const string PermissionNotFound = "Identity.PermissionNotFound";
    public const string CurrentPasswordIncorrect = "Identity.CurrentPasswordIncorrect";
    public const string InvalidCredentials = "Identity.InvalidCredentials";
    public const string AccountLockedOut = "Identity.AccountLockedOut";
    public const string RefreshTokenExpired = "Identity.RefreshTokenExpired";
    public const string InvalidRefreshToken = "Identity.InvalidRefreshToken";
    public const string AccountDeactivated = "Identity.AccountDeactivated";
}
