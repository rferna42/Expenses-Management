namespace FinanceProject.Models;

public sealed class MonthOption
{
    public int Year { get; }
    public int Month { get; }
    public string DisplayName { get; }

    public MonthOption(int year, int month)
    {
        Year = year;
        Month = month;
        DisplayName = new DateTime(year, month, 1).ToString("MMMM yyyy");
    }
}
