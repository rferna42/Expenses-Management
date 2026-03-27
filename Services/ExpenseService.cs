using FinanceProject.Configuration;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class ExpenseService
{
    /// <summary>
    /// Filters transactions by category.
    /// </summary>
    public List<Expense> FilterByCategory(List<Expense> expenses, string category)
    {
        if (category == AppConfiguration.AllCategoriesLabel)
        {
            return expenses.ToList();
        }

        return expenses.Where(x => x.Category == category).ToList();
    }

    /// <summary>
    /// Sorts transactions using the selected sort option.
    /// </summary>
    public List<Expense> SortExpenses(List<Expense> expenses, ExpenseSortOption sortBy)
    {
        return sortBy switch
        {
            ExpenseSortOption.DateAscending => expenses.OrderBy(x => x.Date).ToList(),
            ExpenseSortOption.DateDescending => expenses.OrderByDescending(x => x.Date).ToList(),
            ExpenseSortOption.AmountAscending => expenses.OrderBy(x => x.Amount).ToList(),
            ExpenseSortOption.AmountDescending => expenses.OrderByDescending(x => x.Amount).ToList(),
            ExpenseSortOption.CategoryAscending => expenses.OrderBy(x => x.Category).ToList(),
            _ => expenses.OrderByDescending(x => x.Date).ToList()
        };
    }

    /// <summary>
    /// Applies category filtering and sorting.
    /// </summary>
    public List<Expense> ApplyFilterAndSort(List<Expense> expenses, string category, ExpenseSortOption sortBy)
    {
        var filtered = FilterByCategory(expenses, category);
        return SortExpenses(filtered, sortBy);
    }

    /// <summary>
    /// Validates transaction input.
    /// </summary>
    public bool ValidateExpense(Expense expense, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(expense.Description))
        {
            errorMessage = "Description cannot be empty.";
            return false;
        }

        if (expense.Amount <= 0)
        {
            errorMessage = "Amount must be greater than 0.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

}
