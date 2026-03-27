using System.IO;
using FinanceProject.Configuration;
using FinanceProject.Domain.Repositories;
using FinanceProject.Models;
using Microsoft.Data.Sqlite;

namespace FinanceProject.Infrastructure.Repositories;

public class SqliteExpenseRepository : IExpenseRepository
{
    private readonly string _connectionString;

    public SqliteExpenseRepository()
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppConfiguration.DataFolderName,
            AppConfiguration.DatabaseFileName
        );

        EnsureDirectoryExists(dbPath);
        _connectionString = $"Data Source={dbPath}";
        EnsureDatabase();
    }

    public List<Expense> Load()
    {
        var expenses = new List<Expense>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Description, Amount, Category, Date, TransactionType
            FROM Expenses
            ORDER BY Date DESC;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            expenses.Add(new Expense
            {
                Description = reader.GetString(0),
                Amount = reader.GetDecimal(1),
                Category = reader.GetString(2),
                Date = DateTime.Parse(reader.GetString(3)),
                TransactionType = Enum.Parse<TransactionType>(reader.GetString(4), ignoreCase: true)
            });
        }

        return expenses;
    }

    public void Save(List<Expense> expenses)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        using (var deleteCommand = connection.CreateCommand())
        {
            deleteCommand.Transaction = transaction;
            deleteCommand.CommandText = "DELETE FROM Expenses;";
            deleteCommand.ExecuteNonQuery();
        }

        foreach (var expense in expenses)
        {
            using var insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText = """
                INSERT INTO Expenses (Description, Amount, Category, Date, TransactionType)
                VALUES ($description, $amount, $category, $date, $transactionType);
                """;

            insertCommand.Parameters.AddWithValue("$description", expense.Description);
            insertCommand.Parameters.AddWithValue("$amount", expense.Amount);
            insertCommand.Parameters.AddWithValue("$category", expense.Category);
            insertCommand.Parameters.AddWithValue("$date", expense.Date.ToString("O"));
            insertCommand.Parameters.AddWithValue("$transactionType", expense.TransactionType.ToString());
            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private void EnsureDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Expenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Description TEXT NOT NULL,
                Amount REAL NOT NULL,
                Category TEXT NOT NULL,
                Date TEXT NOT NULL,
                TransactionType TEXT NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_Expenses_Date ON Expenses(Date);
            CREATE INDEX IF NOT EXISTS IX_Expenses_Category ON Expenses(Category);
            CREATE INDEX IF NOT EXISTS IX_Expenses_TransactionType ON Expenses(TransactionType);
            """;
        command.ExecuteNonQuery();
    }

    private static void EnsureDirectoryExists(string dbPath)
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
