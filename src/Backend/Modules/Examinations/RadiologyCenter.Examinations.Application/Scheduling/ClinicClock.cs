using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Scheduling;

/// <summary>
/// The clinic and its patients operate in Egypt regardless of where the server
/// is hosted. Clients submit schedule times as Egypt wall-clock times; this
/// component converts them to true UTC instants so domain guards that compare
/// against DateTime.UtcNow remain correct on any server timezone.
/// </summary>
public static class ClinicClock
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime ToUtc(DateTime egyptWallTime)
        => egyptWallTime.Kind switch
        {
            DateTimeKind.Utc => egyptWallTime,
            DateTimeKind.Local => egyptWallTime.ToUniversalTime(),
            _ => TimeZoneInfo.ConvertTimeToUtc(egyptWallTime, Zone)
        };

    public static DateTime ToLocal(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);

    public static DateTime UtcDayStart(DateTime clinicDate) => ToUtc(DateTime.SpecifyKind(clinicDate.Date, DateTimeKind.Unspecified));
    public static DateTime UtcDayEndExclusive(DateTime clinicDate) => UtcDayStart(clinicDate.Date.AddDays(1));

    private static TimeZoneInfo ResolveZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(TimezoneConstants.DefaultTimezone); }
        catch (TimeZoneNotFoundException)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(TimezoneConstants.WindowsTimezone); }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                throw new InvalidOperationException("The clinic time zone is unavailable.", ex);
            }
        }
    }
}
