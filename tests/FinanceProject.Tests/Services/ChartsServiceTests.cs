using FinanceProject.Models;
using FinanceProject.Services;
using OxyPlot.Series;

namespace FinanceProject.Tests.Services;

public class ChartsServiceTests
{
    private readonly ChartsService _service = new();

    [Fact]
    public void CreateCategoryExpensesChart_ReturnsEmptyModel_WhenThereAreNoExpenses()
    {
        var model = _service.CreateCategoryExpensesChart(new List<Expense>());

        Assert.NotNull(model);
        Assert.Empty(model.Series);
    }

    [Fact]
    public void CreateCategoryExpensesChart_CreatesPieSeries_WhenThereAreExpenses()
    {
        var expenses = new List<Expense>
        {
            new() { Category = "Shopping", TransactionType = TransactionType.Expense, Amount = 50m },
            new() { Category = "Fuel", TransactionType = TransactionType.Expense, Amount = 30m },
            new() { Category = "Income", TransactionType = TransactionType.Income, Amount = 200m }
        };

        var model = _service.CreateCategoryExpensesChart(expenses);

        var pie = Assert.IsType<PieSeries>(Assert.Single(model.Series));
        Assert.Equal(2, pie.Slices.Count);
    }

    [Fact]
    public void CreateIncomeVsExpensesChart_CreatesTwoVerticalBars()
    {
        var today = DateTime.Now;
        var expenses = new List<Expense>
        {
            new() { Date = today, TransactionType = TransactionType.Income, Amount = 1800m },
            new() { Date = today, TransactionType = TransactionType.Expense, Amount = 600m }
        };

        var model = _service.CreateIncomeVsExpensesChart(expenses, today.Year, today.Month);

        var series = Assert.IsType<RectangleBarSeries>(Assert.Single(model.Series));
        Assert.Equal(2, series.Items.Count);
        Assert.Equal(1800d, series.Items[0].Y1);
        Assert.Equal(600d, series.Items[1].Y1);
    }

    [Fact]
    public void CreateIncomeVsExpensesChart_UsesRequestedMonthInsteadOfCurrentMonth()
    {
        var april = new DateTime(2026, 4, 10);
        var march = new DateTime(2026, 3, 12);
        var expenses = new List<Expense>
        {
            new() { Date = april, TransactionType = TransactionType.Income, Amount = 2500m },
            new() { Date = april, TransactionType = TransactionType.Expense, Amount = 900m },
            new() { Date = march, TransactionType = TransactionType.Income, Amount = 100m },
            new() { Date = march, TransactionType = TransactionType.Expense, Amount = 50m }
        };

        var model = _service.CreateIncomeVsExpensesChart(expenses, 2026, 4);

        var series = Assert.IsType<RectangleBarSeries>(Assert.Single(model.Series));
        Assert.Equal(2500d, series.Items[0].Y1);
        Assert.Equal(900d, series.Items[1].Y1);
        Assert.Contains(new DateTime(2026, 4, 1).ToString("MMMM yyyy"), model.Title);
    }
}
