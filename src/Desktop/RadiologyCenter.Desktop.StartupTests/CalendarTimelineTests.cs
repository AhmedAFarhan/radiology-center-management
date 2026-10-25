using RadiologyCenter.Desktop.Features.Examinations.Models;
using Xunit;

namespace RadiologyCenter.Desktop.StartupTests;

public sealed class CalendarTimelineTests
{
    private static readonly DateTime Start = new(2026, 10, 25, 8, 0, 0);

    [Fact]
    public void LongExaminationIsOneBlockWithItsActualOffsetAndDuration()
    {
        var placements = CalendarTimeline.Arrange([new("exam", Start.AddMinutes(10), Start.AddMinutes(85))], Start, Start.AddHours(9));
        var block = Assert.Single(placements);
        Assert.Equal(10, block.StartMinutes);
        Assert.Equal(75, block.DurationMinutes);
    }

    [Fact]
    public void SimultaneousAppointmentsGetSeparateLanesWithoutHidingEither()
    {
        var placements = CalendarTimeline.Arrange([
            new("a", Start, Start.AddMinutes(60)),
            new("b", Start.AddMinutes(10), Start.AddMinutes(40)),
            new("c", Start.AddMinutes(20), Start.AddMinutes(50))], Start, Start.AddHours(9));
        Assert.Equal(3, placements.Count);
        Assert.Equal(3, placements.Select(p => p.Lane).Distinct().Count());
    }

    [Fact]
    public void AdjacentAppointmentsReuseTheLane()
    {
        var placements = CalendarTimeline.Arrange([
            new("a", Start, Start.AddMinutes(30)),
            new("b", Start.AddMinutes(30), Start.AddMinutes(60))], Start, Start.AddHours(9));
        Assert.All(placements, p => Assert.Equal(0, p.Lane));
    }

    [Fact]
    public void CrossingTheVisibleBoundaryClipsTheBlockWithoutNegativeCoordinates()
    {
        var placements = CalendarTimeline.Arrange([
            new("before", Start.AddHours(-1), Start.AddMinutes(20)),
            new("after", Start.AddHours(8), Start.AddHours(10))], Start, Start.AddHours(9));
        Assert.Equal(new TimelinePlacement("before", 0, 20, 0), placements[0]);
        Assert.Equal(new TimelinePlacement("after", 480, 60, 0), placements[1]);
    }

    [Fact]
    public void InvalidAndNonOverlappingAppointmentsDoNotCreatePhantomBlocks()
    {
        var placements = CalendarTimeline.Arrange([
            new("ended", Start.AddHours(-1), Start),
            new("future", Start.AddHours(9), Start.AddHours(10)),
            new("zero", Start, Start),
            new("reversed", Start.AddMinutes(10), Start)], Start, Start.AddHours(9));
        Assert.Empty(placements);
    }

    [Fact]
    public void InputOrderDoesNotChangePlacement()
    {
        TimelineAppointment[] appointments = [new("b", Start, Start.AddMinutes(60)), new("a", Start, Start.AddMinutes(60))];
        Assert.Equal(CalendarTimeline.Arrange(appointments, Start, Start.AddHours(9)),
            CalendarTimeline.Arrange(appointments.Reverse(), Start, Start.AddHours(9)));
    }
}
