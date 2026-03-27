using FinanceProject.Infrastructure.Repositories;
using FinanceProject.Models;
using Microsoft.Data.Sqlite;

namespace FinanceProject.Tests.Repositories;

public sealed class SqliteExpenseRepositoryTests : IDisposable
{
    private readonly string _databasePath;

    public SqliteExpenseRepositoryTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"financeproject-tests-{Guid.NewGuid():N}.db");
    }

    [Fact]
    public void SaveAndLoad_PreservesDecimalAmountsAndIds()
    {
        var repository = new SqliteExpenseRepository(_databasePath);
        var expenses = new List<Expense>
        {
            new()
            {
                Id = "income-1",
                Description = "Salary",
                Amount = 1234.56m,
                Category = "Income",
                Date = new DateTime(2026, 3, 1),
                TransactionType = TransactionType.Income
            },
            new()
            {
                Id = "expense-1",
                Description = "Groceries",
                Amount = 89.45m,
                Category = "Food",
                Date = new DateTime(2026, 3, 2),
                TransactionType = TransactionType.Expense
            }
        };

        repository.Save(expenses);
        var loaded = repository.Load();

        Assert.Equal(2, loaded.Count);
        Assert.Contains(loaded, expense => expense.Id == "income-1" && expense.Amount == 1234.56m);
        Assert.Contains(loaded, expense => expense.Id == "expense-1" && expense.Amount == 89.45m);
    }

    [Fact]
    public void Save_UpdatesExistingRowsAndDeletesMissingRows()
    {
        var repository = new SqliteExpenseRepository(_databasePath);
        repository.Save(
        [
            new Expense
            {
                Id = "keep-me",
                Description = "Initial",
                Amount = 20m,
                Category = "Shopping",
                Date = new DateTime(2026, 1, 1),
                TransactionType = TransactionType.Expense
            },
            new Expense
            {
                Id = "remove-me",
                Description = "To delete",
                Amount = 10m,
                Category = "Fuel",
                Date = new DateTime(2026, 1, 2),
                TransactionType = TransactionType.Expense
            }
        ]);

        repository.Save(
        [
            new Expense
            {
                Id = "keep-me",
                Description = "Updated",
                Amount = 25.75m,
                Category = "Travels",
                Date = new DateTime(2026, 2, 3),
                TransactionType = TransactionType.Expense
            }
        ]);

        var loaded = repository.Load();

        var item = Assert.Single(loaded);
        Assert.Equal("keep-me", item.Id);
        Assert.Equal("Updated", item.Description);
        Assert.Equal(25.75m, item.Amount);
        Assert.Equal("Travels", item.Category);
        Assert.Equal(new DateTime(2026, 2, 3), item.Date);
    }

    [Fact]
    public void Upsert_UpdatesExistingRowWithoutDeletingOthers()
    {
        var repository = new SqliteExpenseRepository(_databasePath);

        repository.Upsert(new Expense
        {
            Id = "existing",
            Description = "Old",
            Amount = 10m,
            Category = "Food",
            Date = new DateTime(2026, 3, 1),
            TransactionType = TransactionType.Expense
        });

        repository.Upsert(new Expense
        {
            Id = "other",
            Description = "Keep",
            Amount = 20m,
            Category = "Fuel",
            Date = new DateTime(2026, 3, 2),
            TransactionType = TransactionType.Expense
        });

        repository.Upsert(new Expense
        {
            Id = "existing",
            Description = "New",
            Amount = 15.50m,
            Category = "Travels",
            Date = new DateTime(2026, 3, 3),
            TransactionType = TransactionType.Expense
        });

        var loaded = repository.Load();

        Assert.Equal(2, loaded.Count);
        Assert.Contains(loaded, expense => expense.Id == "existing" && expense.Description == "New" && expense.Amount == 15.50m);
        Assert.Contains(loaded, expense => expense.Id == "other");
    }

    [Fact]
    public void DeleteById_RemovesOnlyRequestedRow()
    {
        var repository = new SqliteExpenseRepository(_databasePath);
        repository.Upsert(new Expense
        {
            Id = "delete-me",
            Description = "Delete",
            Amount = 50m,
            Category = "Other",
            Date = new DateTime(2026, 3, 4),
            TransactionType = TransactionType.Expense
        });
        repository.Upsert(new Expense
        {
            Id = "keep-me",
            Description = "Keep",
            Amount = 70m,
            Category = "Income",
            Date = new DateTime(2026, 3, 5),
            TransactionType = TransactionType.Income
        });

        repository.DeleteById("delete-me");

        var loaded = repository.Load();

        var remaining = Assert.Single(loaded);
        Assert.Equal("keep-me", remaining.Id);
    }

    [Fact]
    public void Constructor_MigratesOldSchemaToTextAmountAndTextId()
    {
        CreateLegacyDatabase();

        var repository = new SqliteExpenseRepository(_databasePath);
        var loaded = repository.Load();

        var migrated = Assert.Single(loaded);
        Assert.Equal(42.15m, migrated.Amount);
        Assert.False(string.IsNullOrWhiteSpace(migrated.Id));
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }

        var walPath = $"{_databasePath}-wal";
        if (File.Exists(walPath))
        {
            File.Delete(walPath);
        }

        var shmPath = $"{_databasePath}-shm";
        if (File.Exists(shmPath))
        {
            File.Delete(shmPath);
        }
    }

    private void CreateLegacyDatabase()
    {
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE Expenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Description TEXT NOT NULL,
                Amount REAL NOT NULL,
                Category TEXT NOT NULL,
                Date TEXT NOT NULL,
                TransactionType TEXT NOT NULL
            );

            INSERT INTO Expenses (Description, Amount, Category, Date, TransactionType)
            VALUES ('Legacy item', 42.15, 'Food', '2026-03-10T00:00:00.0000000', 'Expense');
            """;
        command.ExecuteNonQuery();
    }
}