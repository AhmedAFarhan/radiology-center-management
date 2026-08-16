namespace RadiologyCenter.Cash.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string OpeningFloatNegative = "Cash.OpeningFloatNegative";
    public const string CloseSessionNotOpen = "Cash.CloseSessionNotOpen";
    public const string CountedTotalNegative = "Cash.CountedTotalNegative";
    public const string ExpectedTotalNegative = "Cash.ExpectedTotalNegative";
    public const string EntryAmountPositive = "Cash.EntryAmountPositive";
}
