using FinanceProject.Configuration;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class ExpenseDashboardService
{
    private readonly ExpenseService _expenseService;
    private readonly MonthlySummaryService _monthlySummaryService;
    private readonly CategoryMappingService _categoryMappingService;

    public ExpenseDashboardService(
        ExpenseService expenseService,
        MonthlySummaryService monthlySummaryService,
        CategoryMappingService categoryMappingService)
    {
        _expenseService = expenseService;
        _monthlySummaryService = monthlySummaryService;
        _categoryMappingService = categoryMappingService;
    }

    public List<MonthOption> GetAvailableMonths(List<Expense> allExpenses, DateTime referenceDate)
    {
        return allExpenses
            .Select(expense => new DateTime(expense.Date.Year, expense.Date.Month, 1))
            .Append(new DateTime(referenceDate.Year, referenceDate.Month, 1))
            .Distinct()
            .OrderByDescending(date => date)
            .Select(date => new MonthOption(date.Year, date.Month))
            .ToList();
    }

    public string ResolveSelectedMonthKey(List<MonthOption> months, string currentMonthKey, DateTime? preferredMonth)
    {
        var preferredKey = preferredMonth.HasValue
            ? BuildMonthKey(preferredMonth.Value.Year, preferredMonth.Value.Month)
            : null;

        if (!string.IsNullOrWhiteSpace(preferredKey)
            && months.Any(month => BuildMonthKey(month.Year, month.Month) == preferredKey))
        {
            return preferredKey;
        }

        if (months.Any(month => BuildMonthKey(month.Year, month.Month) == currentMonthKey))
        {
            return currentMonthKey;
        }

        var fallback = months.FirstOrDefault();
        return fallback is null
            ? BuildMonthKey(DateTime.Today.Year, DateTime.Today.Month)
            : BuildMonthKey(fallback.Year, fallback.Month);
    }

    public List<Expense> GetVisibleExpenses(
        List<Expense> allExpenses,
        MonthOption? selectedMonth,
        string selectedCategory,
        ExpenseSortOption selectedSortOption)
    {
        var monthExpenses = selectedMonth is null
            ? allExpenses
            : allExpenses.Where(expense => expense.Date.Year == selectedMonth.Year && expense.Date.Month == selectedMonth.Month).ToList();

        return _expenseService.ApplyFilterAndSort(monthExpenses.ToList(), selectedCategory, selectedSortOption);
    }

    public MonthlySummary GetSummary(List<Expense> allExpenses, MonthOption? selectedMonth)
    {
        return selectedMonth is null
            ? _monthlySummaryService.GetCurrentMonthSummary(allExpenses)
            : _monthlySummaryService.GetMonthlySummary(allExpenses, selectedMonth.Year, selectedMonth.Month);
    }

    public List<CategoryTotal> GetExpenseCategoryTotals(List<Expense> visibleExpenses)
    {
        return visibleExpenses
            .Where(expense => expense.TransactionType == TransactionType.Expense
                && !_categoryMappingService.IsIncomeCategory(expense.Category))
            .GroupBy(expense => _categoryMappingService.NormalizeCategory(expense.Category))
            .Select(group => new CategoryTotal(group.Key, group.Sum(expense => expense.Amount)))
            .OrderByDescending(item => item.Total)
            .ToList();
    }

    private static string BuildMonthKey(int year, int month) => $"{year:D4}-{month:D2}";
}

public sealed record CategoryTotal(string Category, decimal Total);
