using System.Globalization;

namespace RadiologyCenter.Desktop.Services;

/// <summary>
/// Centralized timezone conversion and date formatting for the Blazor UI.
/// All backend timestamps arrive as UTC; this helper converts them to the user's
/// configured IANA timezone for display.
/// </summary>
public static class TimezoneHelper
{
    private static readonly TimeZoneInfo DefaultTimeZone = ResolveTimeZone("Africa/Cairo");

    /// <summary>
    /// Converts a UTC DateTime to the user's local timezone.
    /// </summary>
    public static DateTime ToUserLocal(DateTime utc, string? timeZoneId)
    {
        var zone = string.IsNullOrWhiteSpace(timeZoneId)
            ? DefaultTimeZone
            : ResolveTimeZone(timeZoneId);

        var specified = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(specified, zone);
    }

    /// <summary>
    /// Converts a UTC DateTime to the user's local timezone and formats it.
    /// </summary>
    public static string Format(DateTime utc, string? timeZoneId, string format = "g")
    {
        var local = ToUserLocal(utc, timeZoneId);
        return local.ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Formats a nullable UTC DateTime. Returns "-" if null.
    /// </summary>
    public static string FormatOptional(DateTime? utc, string? timeZoneId, string format = "g")
    {
        return utc.HasValue ? Format(utc.Value, timeZoneId, format) : "-";
    }

    /// <summary>
    /// Gets the local date in the user's timezone.
    /// </summary>
    public static DateOnly GetLocalDate(DateTime utc, string? timeZoneId)
    {
        return DateOnly.FromDateTime(ToUserLocal(utc, timeZoneId));
    }

    /// <summary>
    /// Gets today's date in the user's timezone.
    /// </summary>
    public static DateOnly GetToday(string? timeZoneId)
    {
        return GetLocalDate(DateTime.UtcNow, timeZoneId);
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return DefaultTimeZone;
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
