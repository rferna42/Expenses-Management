using System.IO;
using System.Globalization;
using FinanceProject.Configuration;
using FinanceProject.Domain.Repositories;
using FinanceProject.Models;
using Microsoft.Data.Sqlite;

namespace FinanceProject.Infrastructure.Repositories;

public class SqliteExpenseRepository : IExpenseRepository
{
    private readonly string _connectionString;

    public SqliteExpenseRepository(string? databasePath = null)
    {
        var dbPath = databasePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppConfiguration.DataFolderName,
            AppConfiguration.DatabaseFileName);

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
            SELECT Id, Description, Amount, Category, Date, TransactionType
            FROM Expenses
            ORDER BY Date DESC;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var amountValue = reader.GetString(2);
            expenses.Add(new Expense
            {
                Id = reader.GetString(0),
                Description = reader.GetString(1),
                Amount = decimal.Parse(amountValue, NumberStyles.Number, CultureInfo.InvariantCulture),
                Category = reader.GetString(3),
                Date = DateTime.Parse(reader.GetString(4), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                TransactionType = Enum.Parse<TransactionType>(reader.GetString(5), ignoreCase: true)
            });
        }

        return expenses;
    }

    public void Save(List<Expense> expenses)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        DeleteMissingExpenses(connection, transaction, expenses);

        foreach (var expense in expenses)
        {
            using var insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText = """
                INSERT INTO Expenses (Id, Description, Amount, Category, Date, TransactionType)
                VALUES ($id, $description, $amount, $category, $date, $transactionType)
                ON CONFLICT(Id) DO UPDATE SET
                    Description = excluded.Description,
                    Amount = excluded.Amount,
                    Category = excluded.Category,
                    Date = excluded.Date,
                    TransactionType = excluded.TransactionType;
                """;

            insertCommand.Parameters.AddWithValue("$id", EnsureExpenseId(expense));
            insertCommand.Parameters.AddWithValue("$description", expense.Description);
            insertCommand.Parameters.AddWithValue("$amount", expense.Amount.ToString(CultureInfo.InvariantCulture));
            insertCommand.Parameters.AddWithValue("$category", expense.Category);
            insertCommand.Parameters.AddWithValue("$date", expense.Date.ToString("O"));
            insertCommand.Parameters.AddWithValue("$transactionType", expense.TransactionType.ToString());
            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public bool ExistsById(string expenseId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Expenses WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", expenseId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void Upsert(Expense expense)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Expenses (Id, Description, Amount, Category, Date, TransactionType)
            VALUES ($id, $description, $amount, $category, $date, $transactionType)
            ON CONFLICT(Id) DO UPDATE SET
                Description = excluded.Description,
                Amount = excluded.Amount,
                Category = excluded.Category,
                Date = excluded.Date,
                TransactionType = excluded.TransactionType;
            """;

        command.Parameters.AddWithValue("$id", EnsureExpenseId(expense));
        command.Parameters.AddWithValue("$description", expense.Description);
        command.Parameters.AddWithValue("$amount", expense.Amount.ToString(CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$category", expense.Category);
        command.Parameters.AddWithValue("$date", expense.Date.ToString("O"));
        command.Parameters.AddWithValue("$transactionType", expense.TransactionType.ToString());
        command.ExecuteNonQuery();
    }

    public void DeleteById(string expenseId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Expenses WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", expenseId);
        command.ExecuteNonQuery();
    }

    private void EnsureDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        if (!TableExists(connection))
        {
            CreateSchema(connection);
            return;
        }

        if (SchemaMigrationRequired(connection))
        {
            MigrateSchema(connection);
        }

        CreateIndexes(connection);
    }

    private static bool TableExists(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Expenses';";
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    private static bool SchemaMigrationRequired(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info(Expenses);";

        using var reader = command.ExecuteReader();
        var amountType = string.Empty;
        var idType = string.Empty;

        while (reader.Read())
        {
            var columnName = reader.GetString(1);
            var columnType = reader.GetString(2);

            if (columnName == "Amount")
            {
                amountType = columnType;
            }

            if (columnName == "Id")
            {
                idType = columnType;
            }
        }

        return !string.Equals(amountType, "TEXT", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(idType, "TEXT", StringComparison.OrdinalIgnoreCase);
    }

    private static void MigrateSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            DROP INDEX IF EXISTS IX_Expenses_Date;
            DROP INDEX IF EXISTS IX_Expenses_Category;
            DROP INDEX IF EXISTS IX_Expenses_TransactionType;

            CREATE TABLE Expenses_Migrated (
                Id TEXT PRIMARY KEY,
                Description TEXT NOT NULL,
                Amount TEXT NOT NULL,
                Category TEXT NOT NULL,
                Date TEXT NOT NULL,
                TransactionType TEXT NOT NULL
            );

            INSERT INTO Expenses_Migrated (Id, Description, Amount, Category, Date, TransactionType)
            SELECT
                COALESCE(CAST(Id AS TEXT), lower(hex(randomblob(16)))),
                Description,
                CAST(Amount AS TEXT),
                Category,
                Date,
                TransactionType
            FROM Expenses;

            DROP TABLE Expenses;
            ALTER TABLE Expenses_Migrated RENAME TO Expenses;
            """;
        command.ExecuteNonQuery();

        CreateIndexes(connection);
    }

    private static void CreateSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Expenses (
                Id TEXT PRIMARY KEY,
                Description TEXT NOT NULL,
                Amount TEXT NOT NULL,
                Category TEXT NOT NULL,
                Date TEXT NOT NULL,
                TransactionType TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();

        CreateIndexes(connection);
    }

    private static void CreateIndexes(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE INDEX IF NOT EXISTS IX_Expenses_Date ON Expenses(Date);
            CREATE INDEX IF NOT EXISTS IX_Expenses_Category ON Expenses(Category);
            CREATE INDEX IF NOT EXISTS IX_Expenses_TransactionType ON Expenses(TransactionType);
            """;
        command.ExecuteNonQuery();
    }

    private static void DeleteMissingExpenses(SqliteConnection connection, SqliteTransaction transaction, List<Expense> expenses)
    {
        using var deleteCommand = connection.CreateCommand();
        deleteCommand.Transaction = transaction;

        if (expenses.Count == 0)
        {
            deleteCommand.CommandText = "DELETE FROM Expenses;";
            deleteCommand.ExecuteNonQuery();
            return;
        }

        var parameterNames = new List<string>();
        for (int index = 0; index < expenses.Count; index++)
        {
            var parameterName = $"$id{index}";
            parameterNames.Add(parameterName);
            deleteCommand.Parameters.AddWithValue(parameterName, EnsureExpenseId(expenses[index]));
        }

        deleteCommand.CommandText = $"DELETE FROM Expenses WHERE Id NOT IN ({string.Join(", ", parameterNames)});";
        deleteCommand.ExecuteNonQuery();
    }

    private static void EnsureDirectoryExists(string dbPath)
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static string EnsureExpenseId(Expense expense)
    {
        if (string.IsNullOrWhiteSpace(expense.Id))
        {
            expense.Id = Guid.NewGuid().ToString("N");
        }

        return expense.Id;
    }
}
