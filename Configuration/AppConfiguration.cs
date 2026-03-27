using System.Globalization;
using FinanceProject.Models;

namespace FinanceProject.Configuration;

public static class AppConfiguration
{
    public const string AllCategoriesLabel = "All";
    public const string DefaultCategory = "Other";
    public const string IncomeCategory = "Income";

    public const string ErrorDialogTitle = "Error";
    public const string DeleteDialogTitle = "Confirm deletion";

    public const string InvalidExpenseMessage = "Please enter a description and a valid amount.";
    public const string InvalidEditMessage = "Please complete all fields correctly.";

    public const string LoadErrorPrefix = "Error loading expenses:";
    public const string SaveErrorPrefix = "Error saving expenses:";
    public const string ChartErrorPrefix = "Error updating chart:";
    public const string IncomeExpenseChartErrorPrefix = "Error updating income vs expenses chart:";
    public const string SummaryErrorPrefix = "Error updating summary:";

    public const string PieChartTitle = "Expense Distribution by Category";
    public const string IncomeVsExpensesChartTitle = "Income vs Expenses";
    public const string AmountAxisTitle = "Amount (EUR)";

    public const string DataFolderName = "FinanceProject";
    public const string DataFileName = "expenses.json";

    public static readonly IReadOnlyList<string> Categories =
    [
        "Shopping",
        "Rent",
        "Dining",
        "Fuel",
        "Loans",
        "Utilities",
        "Entertainment",
        DefaultCategory,
        IncomeCategory
    ];

    public static readonly IReadOnlyList<string> FilterCategories =
    [
        AllCategoriesLabel,
        ..Categories
    ];

    public static readonly IReadOnlyList<TransactionType> TransactionTypes =
    [
        TransactionType.Expense,
        TransactionType.Income
    ];

    public static readonly IReadOnlyList<string> TransactionTypeLabels =
    [
        "Expense",
        "Income"
    ];

    public static readonly IReadOnlyList<SortOptionItem> SortOptions =
    [
        new(ExpenseSortOption.DateDescending, "Date Descending"),
        new(ExpenseSortOption.DateAscending, "Date Ascending"),
        new(ExpenseSortOption.AmountDescending, "Amount Descending"),
        new(ExpenseSortOption.AmountAscending, "Amount Ascending"),
        new(ExpenseSortOption.CategoryAscending, "Category A-Z")
    ];

    public const ExpenseSortOption DefaultSortOption = ExpenseSortOption.DateDescending;

    public static string BuildDeleteConfirmationText(string description, decimal amount)
    {
        return $"Are you sure you want to delete this transaction?\n{description} - {amount:C2}";
    }

    public static bool TryParseAmount(string input, out decimal amount)
    {
        if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out amount))
        {
            return true;
        }

        if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
        {
            return true;
        }

        return decimal.TryParse(input, NumberStyles.Any, new CultureInfo("es-ES"), out amount);
    }
}
