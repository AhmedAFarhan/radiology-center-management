namespace RadiologyCenter.Cash.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string SessionNotFound = "Cash.SessionNotFound";
    public const string HandoverNotFound = "Cash.HandoverNotFound";
    public const string EntryNotOwnSession = "Cash.EntryNotOwnSession";
    public const string CloseNotOwnSession = "Cash.CloseNotOwnSession";
    public const string AddEntryToClosedSession = "Cash.AddEntryToClosedSession";
    public const string HandoverRequiresClosedSession = "Cash.HandoverRequiresClosedSession";
    public const string HandoverAlreadyApproved = "Cash.HandoverAlreadyApproved";
    public const string HandoverApprovedByCloser = "Cash.HandoverApprovedByCloser";
    public const string CloseSessionNotOpen = "Cash.CloseSessionNotOpen";
    public const string ReceiverAlreadyOpenSession = "Cash.ReceiverAlreadyOpenSession";
    public const string SessionAlreadyOpen = "Cash.SessionAlreadyOpen";
    public const string NoActiveTransaction = "Cash.NoActiveTransaction";

    public const string SessionIdRequired = "Cash.SessionIdRequired";
    public const string DirectionInvalid = "Cash.DirectionInvalid";
    public const string ReasonInvalid = "Cash.ReasonInvalid";
    public const string AmountMustBePositive = "Cash.AmountMustBePositive";
    public const string DescriptionTooLong = "Cash.DescriptionTooLong";
    public const string ReferenceIdTooLong = "Cash.ReferenceIdTooLong";
    public const string CountedTotalCannotBeNegative = "Cash.CountedTotalCannotBeNegative";
    public const string ReceivingOpeningFloatCannotBeNegative = "Cash.ReceivingOpeningFloatCannotBeNegative";
    public const string NotesTooLong = "Cash.NotesTooLong";
    public const string OpeningFloatCannotBeNegative = "Cash.OpeningFloatCannotBeNegative";
}
