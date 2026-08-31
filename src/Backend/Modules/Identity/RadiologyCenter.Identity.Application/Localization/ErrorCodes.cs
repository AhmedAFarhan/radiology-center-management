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
    public const string LockoutEndMustBeFuture = "Identity.LockoutEndMustBeFuture";

    public const string UserIdRequired = "Identity.UserIdRequired";
    public const string RoleIdRequired = "Identity.RoleIdRequired";
    public const string PermissionCodeRequired = "Identity.PermissionCodeRequired";
    public const string CurrentPasswordRequired = "Identity.CurrentPasswordRequired";
    public const string NewPasswordRequired = "Identity.NewPasswordRequired";
    public const string PasswordRequired = "Identity.PasswordRequired";
    public const string RoleNameRequired = "Identity.RoleNameRequired";
    public const string RoleNameTooLong = "Identity.RoleNameTooLong";
    public const string UserNameRequired = "Identity.UserNameRequired";
    public const string UserNameTooLong = "Identity.UserNameTooLong";
    public const string EmailRequired = "Identity.EmailRequired";
    public const string EmailInvalid = "Identity.EmailInvalid";
    public const string EmailTooLong = "Identity.EmailTooLong";
    public const string FirstNameRequired = "Identity.FirstNameRequired";
    public const string FirstNameTooLong = "Identity.FirstNameTooLong";
    public const string LastNameRequired = "Identity.LastNameRequired";
    public const string LastNameTooLong = "Identity.LastNameTooLong";
    public const string PhoneNumberRequired = "Identity.PhoneNumberRequired";
    public const string PhoneNumberTooLong = "Identity.PhoneNumberTooLong";
    public const string TimeZoneRequired = "Identity.TimeZoneRequired";
    public const string TimeZoneTooLong = "Identity.TimeZoneTooLong";
    public const string RefreshTokenRequired = "Identity.RefreshTokenRequired";
    public const string TokenRequired = "Identity.TokenRequired";
}
