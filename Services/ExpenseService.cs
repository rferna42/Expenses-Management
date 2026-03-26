using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class ExpenseService
{
    private readonly string _dataFilePath;
    private const string FolderName = "FinanceProject";

    public ExpenseService()
    {
        _dataFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            FolderName,
            "expenses.json"
        );
    }

    /// <summary>
    /// Carga todos los gastos desde el archivo JSON
    /// </summary>
    public List<Expense> LoadExpenses()
    {
        try
        {
            EnsureDirectoryExists();

            if (File.Exists(_dataFilePath))
            {
                var json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<Expense>>(json) ?? [];
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar gastos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return [];
    }

    /// <summary>
    /// Guarda los gastos en archivo JSON
    /// </summary>
    public void SaveExpenses(List<Expense> expenses)
    {
        try
        {
            EnsureDirectoryExists();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(expenses, options);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar gastos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

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

    /// <summary>
    /// Asegura que el directorio de datos existe
    /// </summary>
    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_dataFilePath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
