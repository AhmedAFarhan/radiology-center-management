namespace RadiologyCenter.BuildingBlocks.Domain.Localization;

/// <summary>
/// Strongly-typed shared message codes used as localization keys for the
/// cross-cutting, non-domain messages that live in the host resource files
/// (Resources\en.json / ar.json). Unlike module <c>ErrorCodes</c>, these
/// describe general/guard/validation messages rather than domain errors.
/// </summary>
public static class MessageCodes
{
    public static class Shared
    {
        public const string KeyNotFound = "Shared.KeyNotFound";
        public const string KeyWasNotFound = "Shared.KeyWasNotFound";
        public const string Unauthorized = "Shared.Unauthorized";
        public const string Forbidden = "Shared.Forbidden";
        public const string LockedOut = "Shared.LockedOut";
        public const string ConcurrencyConflict = "Shared.ConcurrencyConflict";
        public const string UnexpectedError = "Shared.UnexpectedError";
        public const string ValidationFailed = "Shared.ValidationFailed";
        public const string InvalidValue = "Shared.InvalidValue";
        public const string InvalidName = "Shared.InvalidName";
        public const string CannotBeNull = "Shared.CannotBeNull";
        public const string CannotBeNullOrWhitespace = "Shared.CannotBeNullOrWhitespace";
        public const string MustBeGreaterThanZero = "Shared.MustBeGreaterThanZero";
        public const string CannotBeDefaultValue = "Shared.CannotBeDefaultValue";
        public const string CannotBeEmpty = "Shared.CannotBeEmpty";
        public const string MustBeOneOf = "Shared.MustBeOneOf";
    }
}