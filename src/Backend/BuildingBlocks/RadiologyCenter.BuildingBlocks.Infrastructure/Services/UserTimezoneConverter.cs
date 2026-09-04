using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class UserTimezoneConverter : ITimezoneConverter
{
    private readonly ICurrentUser _currentUser;
    private readonly TimeZoneInfo _timeZone;

    public UserTimezoneConverter(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
        _timeZone = ResolveTimeZone(currentUser.TimeZoneId ?? TimezoneConstants.DefaultTimezone);
    }

    public string TimeZoneId => _timeZone.Id;

    public DateTime ToLocal(DateTime utc)
    {
        var specified = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(specified, _timeZone);
    }

    public DateTime ToUtc(DateTime local)
    {
        var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, _timeZone);
    }

    public DateOnly GetLocalDate(DateTime utc) => DateOnly.FromDateTime(ToLocal(utc));

    public (DateTime FromUtc, DateTime ToUtcExclusive) GetLocalDateBounds(DateOnly localDate)
    {
        var localStart = localDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        var localEnd = localDate.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);

        return (ToUtc(localStart), ToUtc(localEnd));
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimezoneConstants.DefaultTimezone);
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
