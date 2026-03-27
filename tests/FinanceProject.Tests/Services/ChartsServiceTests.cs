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

        var model = _service.CreateIncomeVsExpensesChart(expenses);

        var series = Assert.IsType<RectangleBarSeries>(Assert.Single(model.Series));
        Assert.Equal(2, series.Items.Count);
        Assert.Equal(1800d, series.Items[0].Y1);
        Assert.Equal(600d, series.Items[1].Y1);
    }
}
