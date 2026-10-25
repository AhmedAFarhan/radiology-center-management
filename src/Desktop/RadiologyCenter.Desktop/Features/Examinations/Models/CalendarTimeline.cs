namespace RadiologyCenter.Desktop.Features.Examinations.Models;

public sealed record TimelineAppointment(string Id, DateTime Start, DateTime End);
public sealed record TimelinePlacement(string Id, double StartMinutes, double DurationMinutes, int Lane);

public static class CalendarTimeline
{
    public static IReadOnlyList<TimelinePlacement> Arrange(IEnumerable<TimelineAppointment> appointments, DateTime start, DateTime end)
    {
        var result = new List<TimelinePlacement>();
        var laneEnds = new List<DateTime>();
        foreach (var item in appointments.Where(a => a.End > start && a.Start < end && a.End > a.Start)
                     .OrderBy(a => a.Start).ThenBy(a => a.End).ThenBy(a => a.Id, StringComparer.Ordinal))
        {
            var clippedStart = item.Start < start ? start : item.Start;
            var clippedEnd = item.End > end ? end : item.End;
            var lane = laneEnds.FindIndex(previousEnd => previousEnd <= clippedStart);
            if (lane < 0) { lane = laneEnds.Count; laneEnds.Add(clippedEnd); }
            else laneEnds[lane] = clippedEnd;
            result.Add(new(item.Id, (clippedStart - start).TotalMinutes, (clippedEnd - clippedStart).TotalMinutes, lane));
        }
        return result;
    }
}
