
namespace RadiologyCenter.Desktop.Services;

public static class StatusTokens
{
    private static readonly Dictionary<string, Color> Tokens = new(StringComparer.Ordinal)
    {
        ["Active"] = Color.Success,
        ["Available"] = Color.Success,
        ["Completed"] = Color.Success,
        ["Received"] = Color.Success,
        ["Paid"] = Color.Success,
        ["Approved"] = Color.Success,
        ["Operational"] = Color.Success,
        ["Open"] = Color.Success,
        ["Sent"] = Color.Success,
        ["Finalized"] = Color.Success,
        ["Receive"] = Color.Success,
        ["In"] = Color.Success,

        ["InProgress"] = Color.Warning,
        ["Pending"] = Color.Warning,
        ["UnderMaintenance"] = Color.Warning,
        ["PartiallyReceived"] = Color.Warning,
        ["Urgent"] = Color.Warning,
        ["Locked"] = Color.Warning,
        ["Issue"] = Color.Warning,

        ["Computed"] = Color.Info,
        ["Submitted"] = Color.Info,
        ["CheckedIn"] = Color.Info,
        ["Adjustment"] = Color.Info,
        ["Mild"] = Color.Info,

        ["Ordered"] = Color.Primary,
        ["Scheduled"] = Color.Primary,
        ["Monthly"] = Color.Primary,
        ["Fixed"] = Color.Primary,

        ["New"] = Color.Secondary,
        ["ReturnToSupplier"] = Color.Secondary,
        ["Hourly"] = Color.Secondary,
        ["Other"] = Color.Secondary,

        ["Annual"] = Color.Success,
        ["Maternity"] = Color.Info,
        ["Sick"] = Color.Warning,
        ["Unpaid"] = Color.Default,

        ["Stat"] = Color.Error,
        ["Severe"] = Color.Error,
        ["Denied"] = Color.Error,
        ["Rejected"] = Color.Error,
        ["Failed"] = Color.Error,
        ["OutOfService"] = Color.Error,
        ["Disposal"] = Color.Error,
        ["Out"] = Color.Error,
        ["Cancelled"] = Color.Error,

        ["Draft"] = Color.Default,
        ["Retired"] = Color.Default,
        ["Closed"] = Color.Default,
        ["Inactive"] = Color.Default,
    };

    public static Color For(string? statusKey)
        => statusKey is not null && Tokens.TryGetValue(statusKey, out var color) ? color : Color.Default;
}
