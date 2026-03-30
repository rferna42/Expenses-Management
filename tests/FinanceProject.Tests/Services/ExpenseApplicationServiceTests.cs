using FinanceProject.Domain.Repositories;
using FinanceProject.Models;
using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class ExpenseApplicationServiceTests
{
    [Fact]
    public void UpdateExpense_ReturnsFalse_WhenExpenseDoesNotExist()
    {
        var repository = new FakeExpenseRepository();
        var service = new ExpenseApplicationService(repository);

        var result = service.UpdateExpense(new Expense { Id = "missing" });

        Assert.False(result);
    }

    [Fact]
    public void AddExpense_AndDeleteExpense_WorkAsExpected()
    {
        var repository = new FakeExpenseRepository();
        var service = new ExpenseApplicationService(repository);

        var expense = new Expense
        {
            Id = "e1",
            Description = "Lunch",
            Amount = 12m,
            Category = "Restaurantes",
            Date = new DateTime(2026, 3, 1),
            TransactionType = TransactionType.Expense
        };

        service.AddExpense(expense);
        var deleted = service.DeleteExpense("e1");

        Assert.True(deleted);
        Assert.Empty(repository.Expenses);
    }

    private sealed class FakeExpenseRepository : IExpenseRepository
    {
        public List<Expense> Expenses { get; } = [];

        public List<Expense> Load() => Expenses.ToList();

        public void Save(List<Expense> expenses)
        {
            Expenses.Clear();
            Expenses.AddRange(expenses);
        }

        public bool ExistsById(string expenseId) => Expenses.Any(expense => expense.Id == expenseId);

        public void Upsert(Expense expense)
        {
            var existing = Expenses.FirstOrDefault(item => item.Id == expense.Id);
            if (existing is null)
            {
                Expenses.Add(expense);
                return;
            }

            existing.Description = expense.Description;
            existing.Amount = expense.Amount;
            existing.Category = expense.Category;
            existing.Date = expense.Date;
            existing.TransactionType = expense.TransactionType;
        }

        public void DeleteById(string expenseId)
        {
            Expenses.RemoveAll(expense => expense.Id == expenseId);
        }
    }
}
