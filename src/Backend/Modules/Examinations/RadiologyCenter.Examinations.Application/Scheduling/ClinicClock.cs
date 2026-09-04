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
        => TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(egyptWallTime, DateTimeKind.Unspecified),
            Zone);

    private static TimeZoneInfo ResolveZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(TimezoneConstants.DefaultTimezone); }
        catch (TimeZoneNotFoundException)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(TimezoneConstants.WindowsTimezone); }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                return TimeZoneInfo.Utc;
            }
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
