namespace RadiologyCenter.Payroll.Domain.Common;

public static class PayrollCalendar
{
    public const DayOfWeek FirstWeekendDay = DayOfWeek.Friday;
    public const DayOfWeek SecondWeekendDay = DayOfWeek.Saturday;

    public static bool IsWorkingDay(DateTime date)
        => date.DayOfWeek != FirstWeekendDay && date.DayOfWeek != SecondWeekendDay;

    public static int WorkingDaysBetween(DateTime from, DateTime to)
    {
        if (to.Date < from.Date)
            return 0;

        var fromDate = from.Date;
        var toDate = to.Date;

        int count = 0;
        for (var day = fromDate; day <= toDate; day = day.AddDays(1))
        {
            if (IsWorkingDay(day))
                count++;
        }

        return count;
    }
}