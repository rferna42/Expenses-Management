using System.Text.Json;
using System.IO;
using FinanceProject.Configuration;
using FinanceProject.Domain.Repositories;
using FinanceProject.Models;

namespace FinanceProject.Infrastructure.Repositories;

public class JsonExpenseRepository : IExpenseRepository
{
    private readonly string _dataFilePath;
    public JsonExpenseRepository()
    {
        _dataFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppConfiguration.DataFolderName,
            AppConfiguration.DataFileName
        );
    }

    public List<Expense> Load()
    {
        EnsureDirectoryExists();

        if (!File.Exists(_dataFilePath))
        {
            return [];
        }

        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<Expense>>(json) ?? [];
    }

    public void Save(List<Expense> expenses)
    {
        EnsureDirectoryExists();

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(expenses, options);
        File.WriteAllText(_dataFilePath, json);
    }

    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_dataFilePath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
