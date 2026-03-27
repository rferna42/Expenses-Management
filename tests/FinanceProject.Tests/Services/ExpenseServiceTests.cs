using FinanceProject.Models;
using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class ExpenseServiceTests
{
    private readonly ExpenseService _service = new();

    [Fact]
    public void ValidateExpense_ReturnsFalse_WhenDescriptionIsEmpty()
    {
        var expense = new Expense { Description = "", Amount = 10m };

        var isValid = _service.ValidateExpense(expense, out var errorMessage);

        Assert.False(isValid);
        Assert.Equal("Description cannot be empty.", errorMessage);
    }

    [Fact]
    public void ValidateExpense_ReturnsFalse_WhenAmountIsZeroOrLess()
    {
        var expense = new Expense { Description = "Cafe", Amount = 0m };

        var isValid = _service.ValidateExpense(expense, out var errorMessage);

        Assert.False(isValid);
        Assert.Equal("Amount must be greater than 0.", errorMessage);
    }

    [Fact]
    public void ValidateExpense_ReturnsTrue_WhenExpenseIsValid()
    {
        var expense = new Expense { Description = "Supermercado", Amount = 45.5m };

        var isValid = _service.ValidateExpense(expense, out var errorMessage);

        Assert.True(isValid);
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void FilterByCategory_ReturnsAll_WhenCategoryIsAll()
    {
        var expenses = BuildExpenses();

        var result = _service.FilterByCategory(expenses, "All");

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void ApplyFilterAndSort_FiltersByCategory_AndSortsByAmountDescending()
    {
        var expenses = BuildExpenses();

        var result = _service.ApplyFilterAndSort(expenses, "Shopping", ExpenseSortOption.AmountDescending);

        Assert.Equal(2, result.Count);
        Assert.Equal(25m, result[0].Amount);
        Assert.Equal(10m, result[1].Amount);
    }

    private static List<Expense> BuildExpenses()
    {
        return
        [
            new Expense { Description = "A", Category = "Shopping", Amount = 10m, Date = new DateTime(2026, 3, 1) },
            new Expense { Description = "B", Category = "Shopping", Amount = 25m, Date = new DateTime(2026, 3, 2) },
            new Expense { Description = "C", Category = "Fuel", Amount = 20m, Date = new DateTime(2026, 3, 3) }
        ];
    }
}
