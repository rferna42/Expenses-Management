using FinanceProject.Models;
using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class ExpenseDashboardServiceTests
{
    private readonly ExpenseDashboardService _service = new(new ExpenseService(), new MonthlySummaryService(), new CategoryMappingService());

    [Fact]
    public void GetExpenseCategoryTotals_ExcludesIncomeCategories_AndGroupsTotals()
    {
        var expenses = new List<Expense>
        {
            new() { Category = "Compras", Amount = 20m, TransactionType = TransactionType.Expense },
            new() { Category = "Compras", Amount = 15m, TransactionType = TransactionType.Expense },
            new() { Category = "Viajes", Amount = 30m, TransactionType = TransactionType.Expense },
            new() { Category = "Ingresos", Amount = 2000m, TransactionType = TransactionType.Expense },
            new() { Category = "Income", Amount = 1500m, TransactionType = TransactionType.Expense },
            new() { Category = "Ingresos", Amount = 2500m, TransactionType = TransactionType.Income }
        };

        var totals = _service.GetExpenseCategoryTotals(expenses);

        Assert.Equal(2, totals.Count);
        Assert.Equal("Compras", totals[0].Category);
        Assert.Equal(35m, totals[0].Total);
        Assert.Equal("Viajes", totals[1].Category);
        Assert.Equal(30m, totals[1].Total);
    }
}