namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Converts between UTC and the current user's local timezone.
/// All persisted timestamps are UTC; this service handles display conversion.
/// </summary>
public interface ITimezoneConverter
{
    /// <summary>
    /// Gets the current user's IANA timezone ID (e.g. "Africa/Cairo").
    /// </summary>
    string TimeZoneId { get; }

    /// <summary>
    /// Converts a UTC DateTime to the user's local timezone.
    /// </summary>
    DateTime ToLocal(DateTime utc);

    /// <summary>
    /// Converts a local DateTime (assumed to be in the user's timezone) to UTC.
    /// </summary>
    DateTime ToUtc(DateTime local);

    /// <summary>
    /// Gets the local date in the user's timezone for a given UTC timestamp.
    /// </summary>
    DateOnly GetLocalDate(DateTime utc);

    /// <summary>
    /// Gets the start and end of a local date (in the user's timezone) as UTC bounds.
    /// Useful for "today" or date-range queries against the database.
    /// </summary>
    (DateTime FromUtc, DateTime ToUtcExclusive) GetLocalDateBounds(DateOnly localDate);
}
