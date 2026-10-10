namespace RadiologyCenter.Desktop.Services;

public static class ClinicTime
{
    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);
    public static DateTime Today => Now.Date;
    public static DateTime ToUtc(DateTime wallTime) => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(wallTime, DateTimeKind.Unspecified), Zone);
    public static DateTime ToLocal(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);
}
