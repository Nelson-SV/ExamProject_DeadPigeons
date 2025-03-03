using System.Globalization;

namespace Service;

public class WeekExtractor
{
    public (int WeekNumber, int Year) GetWeekNumberAndYear(DateTime date)
    {
        var cultureInfo = CultureInfo.CurrentCulture;
        var calendar = cultureInfo.Calendar;
        var weekNumber = calendar.GetWeekOfYear(date, cultureInfo.DateTimeFormat.CalendarWeekRule, cultureInfo.DateTimeFormat.FirstDayOfWeek);
        var year = date.Year;
        return (weekNumber, year);
    }
}