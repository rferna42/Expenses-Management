using FinanceProject.Models;
using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class MonthlySummaryServiceTests
{
    private readonly MonthlySummaryService _service = new();

    [Fact]
    public void GetMonthlySummary_CalculatesIncomeExpensesAndBalance_ForGivenMonth()
    {
        var expenses = new List<Expense>
        {
            new() { Date = new DateTime(2026, 3, 2), TransactionType = TransactionType.Income, Amount = 2000m },
            new() { Date = new DateTime(2026, 3, 5), TransactionType = TransactionType.Expense, Amount = 350m },
            new() { Date = new DateTime(2026, 3, 10), TransactionType = TransactionType.Expense, Amount = 150m },
            new() { Date = new DateTime(2026, 2, 10), TransactionType = TransactionType.Expense, Amount = 999m }
        };

        var summary = _service.GetMonthlySummary(expenses, 2026, 3);

        Assert.Equal(2026, summary.Year);
        Assert.Equal(3, summary.Month);
        Assert.Equal(2000m, summary.TotalIncome);
        Assert.Equal(500m, summary.TotalExpenses);
        Assert.Equal(1500m, summary.Balance);
    }

    [Fact]
    public void GetCurrentMonthSummary_UsesCurrentMonthData()
    {
        var today = DateTime.Now;
        var expenses = new List<Expense>
        {
            new() { Date = new DateTime(today.Year, today.Month, 1), TransactionType = TransactionType.Income, Amount = 1000m },
            new() { Date = new DateTime(today.Year, today.Month, 2), TransactionType = TransactionType.Expense, Amount = 200m },
            new() { Date = today.AddMonths(-1), TransactionType = TransactionType.Expense, Amount = 999m }
        };

        var summary = _service.GetCurrentMonthSummary(expenses);

        Assert.Equal(today.Year, summary.Year);
        Assert.Equal(today.Month, summary.Month);
        Assert.Equal(1000m, summary.TotalIncome);
        Assert.Equal(200m, summary.TotalExpenses);
        Assert.Equal(800m, summary.Balance);
    }

    [Fact]
    public void GetLast12MonthsSummary_ReturnsTwelveItemsOrderedOldestToNewest()
    {
        var summaries = _service.GetLast12MonthsSummary(new List<Expense>());

        Assert.Equal(12, summaries.Count);
        Assert.True(summaries[0].Year <= summaries[^1].Year);
    }
}
