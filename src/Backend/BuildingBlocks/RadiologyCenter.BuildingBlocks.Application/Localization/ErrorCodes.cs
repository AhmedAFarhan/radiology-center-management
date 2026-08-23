namespace RadiologyCenter.BuildingBlocks.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Each module
/// owns its own <c>ErrorCodes</c> class in its Application project; only the
/// cross-cutting <c>Shared</c> codes live in the Building Blocks.
/// </summary>
public static class ErrorCodes
{
    public static class Shared
    {
        public const string IdRequired = "Shared.IdRequired";
        public const string FieldRequired = "Shared.FieldRequired";
        public const string ValueMustBePositive = "Shared.ValueMustBePositive";
        public const string CannotBeNegative = "Shared.CannotBeNegative";
        public const string TextTooLong = "Shared.TextTooLong";
        public const string InvalidEmail = "Shared.InvalidEmail";
        public const string MustBeBetween = "Shared.MustBeBetween";
        public const string PasswordPolicy = "Shared.PasswordPolicy";
        public const string FullNameTwoParts = "Shared.FullNameTwoParts";
        public const string InvalidPhoneNumber = "Shared.InvalidPhoneNumber";
        public const string InvalidEnumValue = "Shared.InvalidEnumValue";
    }
}
