using FinanceProject.Models;

namespace FinanceProject.Services;

public class ExpenseService
{
    /// <summary>
    /// Filtra gastos por categoría
    /// </summary>
    public List<Expense> FilterByCategory(List<Expense> expenses, string category)
    {
        if (category == "Todas")
        {
            return expenses.ToList();
        }

        return expenses.Where(x => x.Category == category).ToList();
    }

    /// <summary>
    /// Ordena gastos según el criterio especificado
    /// </summary>
    public List<Expense> SortExpenses(List<Expense> expenses, string sortBy)
    {
        return sortBy switch
        {
            "Fecha Ascendente" => expenses.OrderBy(x => x.Date).ToList(),
            "Fecha Descendente" => expenses.OrderByDescending(x => x.Date).ToList(),
            "Cantidad Ascendente" => expenses.OrderBy(x => x.Amount).ToList(),
            "Cantidad Descendente" => expenses.OrderByDescending(x => x.Amount).ToList(),
            "Categoría A-Z" => expenses.OrderBy(x => x.Category).ToList(),
            _ => expenses.OrderByDescending(x => x.Date).ToList()
        };
    }

    /// <summary>
    /// Aplica filtro y ordenamiento a los gastos
    /// </summary>
    public List<Expense> ApplyFilterAndSort(List<Expense> expenses, string category, string sortBy)
    {
        var filtered = FilterByCategory(expenses, category);
        return SortExpenses(filtered, sortBy);
    }

    /// <summary>
    /// Valida que un gasto sea válido
    /// </summary>
    public bool ValidateExpense(Expense expense, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(expense.Description))
        {
            errorMessage = "La descripción no puede estar vacía.";
            return false;
        }

        if (expense.Amount <= 0)
        {
            errorMessage = "El monto debe ser mayor a 0.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

}
