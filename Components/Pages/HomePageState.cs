using FinanceProject.Configuration;
using FinanceProject.Models;

namespace FinanceProject.Components.Pages;

public sealed class HomePageState
{
    public Expense NewExpense { get; private set; } = CreateNewExpense();
    public Expense? EditingExpense { get; private set; }
    public string NewExpenseTypeText { get; set; } = TransactionType.Expense.ToString();
    public string EditingExpenseTypeText { get; set; } = TransactionType.Expense.ToString();

    public void AlignNewExpenseDate(DateTime selectedMonthDate)
    {
        NewExpense.Date = selectedMonthDate;
    }

    public Expense BuildNewExpense(Func<string, TransactionType> parseTransactionType)
    {
        return new Expense
        {
            Description = NewExpense.Description.Trim(),
            Amount = NewExpense.Amount,
            Category = NewExpense.Category,
            Date = NewExpense.Date,
            TransactionType = parseTransactionType(NewExpenseTypeText)
        };
    }

    public void ResetNewExpense(DateTime date)
    {
        NewExpense = CreateNewExpense(date);
        NewExpenseTypeText = TransactionType.Expense.ToString();
    }

    public void BeginEdit(Expense expense)
    {
        EditingExpense = CloneExpense(expense);
        EditingExpenseTypeText = EditingExpense.TransactionType.ToString();
    }

    public Expense? BuildEditedExpense(Func<string, TransactionType> parseTransactionType)
    {
        if (EditingExpense is null)
        {
            return null;
        }

        var clone = CloneExpense(EditingExpense);
        clone.Description = clone.Description.Trim();
        clone.TransactionType = parseTransactionType(EditingExpenseTypeText);
        return clone;
    }

    public void CancelEdit()
    {
        EditingExpense = null;
    }

    private static Expense CloneExpense(Expense expense)
    {
        return new Expense
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date,
            TransactionType = expense.TransactionType
        };
    }

    private static Expense CreateNewExpense(DateTime? date = null)
    {
        return new Expense
        {
            Date = date ?? DateTime.Today,
            Category = AppConfiguration.Categories[0],
            TransactionType = TransactionType.Expense
        };
    }
}
